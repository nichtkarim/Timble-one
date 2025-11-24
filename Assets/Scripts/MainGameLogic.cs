using UnityEngine;
using System.Collections;
using System.Linq;
using Unity.VisualScripting;
using System.Threading.Tasks;
using UnityEngine.InputSystem;


public class MainGameLogic : MonoBehaviour
{
    public Transform[] cups;
    private Vector3[] startPositions;
    public Transform ball;

    private Transform correctCup;
    public static MainGameLogic Instance;
    public HealthSystem Player = new HealthSystem(); 
    public IntuitionSystem Intuition = new IntuitionSystem();
    public HealthSystem Dealer = new HealthSystem();
    private TaskCompletionSource<Cup> clickTaskSource;
    
    public static bool PlayerCanClick = false;
    [SerializeField] private float baseShuffleSpeed = 2f;
    [HideInInspector] public float roundBasedModifier = 1f;
    [SerializeField] private float speedIncreasePerRound = 0.2f;
    
    // Bluff-System
    private bool playerWantsToBluff = false;
    private bool dealerRemovedBallThisRound = false;
    
    
    void Awake()
    {
        Instance = this;
        startPositions = cups.Select(c => c.position).ToArray();
    }

    void Update()
    {
        // F-Taste zum Bluff callen (nur wenn Spieler wählen kann)
        if (PlayerCanClick && Keyboard.current != null && Keyboard.current.fKey.wasPressedThisFrame)
        {
            Debug.Log("⌨️ F-Taste gedrückt - Bluff wird gecallt!");
            OnBluffCalled();
        }
    }

    async void Start()
    {
        Debug.Log("Willkommen zum Becher-Spiel!");
        await GameLoop();
    }

private async Task GameLoop()
{
        while (Player.getCurrentHealth() > 0 && Dealer.getCurrentHealth() > 0)
        {
             // 🪄 Neue Runde vorbereiten
        InventorySystem.Instance.RefillSlots();

        // Hier kannst du z. B. warten, bis Spieler Items auswählt
           await WaitForItemUsePhase();
            await NewRoundAsync();
            baseShuffleSpeed += speedIncreasePerRound;
            roundBasedModifier = 1f;
        }
        if (Player.getCurrentHealth() <= 0)
        {
            Debug.Log("Der Dealer gewinnt!");
        }
        else
        {
            Debug.Log("Der Spieler gewinnt!");
        }
}
    /*
    public void OnCorrectCupSelected(Cup cup)
    {
        Debug.Log(" Richtiger Cup ausgewaehlt: " + cup.name);
        Dealer.takeDamage(1);
    }

    public void OnWrongCupSelected(Cup cup)
    {
        Debug.Log(" Falscher Cup: " + cup.name);

        Player.takeDamage(1);
    }
    */
    public void OnCupSelected(Cup cup)
    {
        if (cup.isCorrectCup)
        {
            Debug.Log(" Richtiger Cup ausgewaehlt: " + cup.name);
            
            // Visuelles Feedback für richtigen Cup
            if (VisualFeedbackManager.Instance != null)
            {
                Debug.Log("✅ VisualFeedbackManager gefunden, triggere Feedback...");
                VisualFeedbackManager.Instance.TriggerCorrectCupFeedback(cup.transform.position, cup.transform);
            }
            else
            {
                Debug.LogError("❌ VisualFeedbackManager.Instance ist NULL! Hast du das GameObject in der Szene?");
            }
            
            Dealer.takeDamage(1);
        }
        else
        {
            Debug.Log(" Falscher Cup: " + cup.name);
            
            // === INTUITION-VERLUST bei falschem Cup ===
            if (IntuitionSystem.Instance != null)
            {
                IntuitionSystem.Instance.OnWrongCupSelected();
            }
            
            // Visuelles Feedback für falschen Cup
            if (VisualFeedbackManager.Instance != null)
            {
                Debug.Log("✅ VisualFeedbackManager gefunden, triggere Feedback...");
                VisualFeedbackManager.Instance.TriggerWrongCupFeedback(cup.transform.position, cup.transform);
            }
            else
            {
                Debug.LogError("❌ VisualFeedbackManager.Instance ist NULL! Hast du das GameObject in der Szene?");
            }

            Player.takeDamage(1);
        }

        clickTaskSource?.TrySetResult(cup);

    }


    private async Task NewRoundAsync()
    {
        Debug.Log("Neue Runde gestartet!");
        
        // === SCHRITT 1: DEALER ENTSCHEIDET OB ER SCHUMMELT (25% Chance) ===
        if (IntuitionSystem.Instance != null)
        {
            dealerRemovedBallThisRound = IntuitionSystem.Instance.ShouldRemoveBall();
        }
        
        // === SCHRITT 2: BALL NORMAL PLATZIEREN (immer!) ===
        correctCup = cups[Random.Range(0, cups.Length)];
        correctCup.GetComponent<Cup>().isCorrectCup = true;
        
        ball.position = new Vector3(correctCup.position.x, ball.position.y, ball.position.z);
        ball.gameObject.SetActive(true);

        await MoveAllCupsDown(cups, -0.4f, 0.7f);

        ball.parent = correctCup;
        
        PlayerCanClick = false;
        await shuffleCups();
        
        // === SCHRITT 4: INTUITION GIBT TIPP OB GESCHUMMELT WURDE ===
        if (IntuitionSystem.Instance != null)
        {
            // Tipp-System: Spieler erfährt OB Dealer geschummelt hat (basierend auf Intuition)
            IntuitionSystem.Instance.GiveCheatingTip();
            
            // Zeige Info wenn Tipp erhalten wurde
            if (IntuitionSystem.Instance.HasCheatingTip())
            {
                Debug.Log("💡 SPIELER HAT TIPP ERHALTEN: Dealer hat geschummelt!");
                
                // UI-Text anzeigen
                if (BluffButton.Instance != null)
                {
                    BluffButton.Instance.ShowTipInfo();
                }
            }
            else
            {
                Debug.Log($"❌ Kein Tipp erhalten (Intuition: {IntuitionSystem.Instance.getCurrentIntuition()}%)");
            }
        }
        
        PlayerCanClick = true; 
        ball.parent = null;

        Cup chosenCup = await WaitForCupClick();
        
        // === BLUFF-LOGIK ===
        // Prüfe ob Spieler blufft (F-Taste wurde gedrückt)
        if (playerWantsToBluff)
        {
            Debug.Log("🃏 Spieler callt BLUFF (F-Taste) - behauptet Dealer hat geschummelt!");
            HandleBluff();
            
            // Ball reaktivieren falls entfernt
            if (dealerRemovedBallThisRound)
            {
                ball.gameObject.SetActive(true);
                Debug.Log("🎱 Ball wird wieder aktiviert für nächste Runde");
            }
            
            // Runde endet nach Bluff
            IntuitionSystem.Instance?.OnRoundEnd();
            ResetCupPositions();
            return;
        }
        
        // === Ball wieder aktivieren falls er entfernt wurde ===
        if (dealerRemovedBallThisRound)
        {
            ball.gameObject.SetActive(true);
            Debug.Log("🎱 Ball wird wieder aktiviert für nächste Runde");
        }
        
        if (chosenCup.isCorrectCup)
        {
            await moveUpOrDown(chosenCup.transform, 0.4f, 0.5f);
        }
        else
        {
            await moveUpOrDown(chosenCup.transform, 0.4f, 0.5f);
            
            // Zeige richtigen Cup nur wenn Ball nicht entfernt wurde
            if (correctCup != null)
            {
                await moveUpOrDown(correctCup, 0.4f, 0.5f);
            }
            else
            {
                Debug.Log("💀 Kein Cup war richtig - Ball wurde entfernt!");
            }
        }

        Debug.Log("Auswahl erkannt → Runde vorbei");
        ResetCupPositions();
        
        // === SCHRITT 7: INTUITION VERLUST AM RUNDEN-ENDE ===
        if (IntuitionSystem.Instance != null)
        {
            IntuitionSystem.Instance.OnRoundEnd();
            Debug.Log($"📉 Runde beendet - Intuition jetzt: {IntuitionSystem.Instance.getCurrentIntuition()}%");
        }
    }

    // === BLUFF-SYSTEM METHODEN ===
    
    /// <summary>
    /// Wird aufgerufen wenn F-Taste gedrückt wird oder Bluff-Button geklickt
    /// </summary>
    public void OnBluffCalled()
    {
        playerWantsToBluff = true;
        Debug.Log("🎯 Bluff-Flag gesetzt!");
        
        // Trigger den Cup-Click Task damit das Spiel weitergeht
        // Wir geben einfach irgendeinen Cup zurück, wird eh ignoriert bei Bluff
        if (clickTaskSource != null && !clickTaskSource.Task.IsCompleted)
        {
            clickTaskSource.TrySetResult(null);
        }
    }
    
    /// <summary>
    /// Verarbeitet Bluff-Logik - Spieler behauptet Dealer hat geschummelt
    /// </summary>
    private void HandleBluff()
    {
        bool bluffWasCorrect;
        IntuitionSystem.Instance.CallBluff(out bluffWasCorrect);
        
        if (bluffWasCorrect)
        {
            // Dealer HAT geschummelt - Dealer verliert 2 Leben
            Debug.Log("✅ BLUFF RICHTIG! Dealer hat geschummelt → Dealer verliert 2 Leben!");
            
            if (Dealer != null)
            {
                Dealer.takeDamage(2);
            }
            
            // Grüner Flash
            if (VisualFeedbackManager.Instance != null)
            {
                VisualFeedbackManager.Instance.FlashScreen(Color.green, 0.5f, 0.4f);
            }
        }
        else
        {
            // Dealer hat NICHT geschummelt - Spieler verliert 2 Leben
            Debug.Log("❌ BLUFF FALSCH! Dealer war ehrlich → Spieler verliert 2 Leben!");
            
            if (Player != null)
            {
                Player.takeDamage(2);
            }
            
            // Roter Flash
            if (VisualFeedbackManager.Instance != null)
            {
                VisualFeedbackManager.Instance.FlashScreen(Color.red, 0.5f, 0.4f);
            }
        }
        
        playerWantsToBluff = false;
    }

async Task shuffleCups()
{
    for (int i = 0; i < 6; i++)
    {
        Transform cupA = cups[Random.Range(0, cups.Length)];
        Transform cupB = cups[Random.Range(0, cups.Length)];

        Vector3 posA = cupA.position;
        Vector3 posB = cupB.position;

        float t = 0;
        while (t < 1)
        {
            cupA.position = Vector3.Lerp(posA, posB, t);
            cupB.position = Vector3.Lerp(posB, posA, t);
          
            t += Time.deltaTime * baseShuffleSpeed * roundBasedModifier; 
            await Task.Yield();
        }
        
        // === DEALER ENTFERNT BALL HEIMLICH WÄHREND DES MISCHENS ===
        if (dealerRemovedBallThisRound && i == 3) // In der Mitte des Mischens
        {
            Debug.Log("🎲 Dealer entfernt Ball heimlich während des Mischens!");
            
            if (ball != null)
            {
                ball.parent = null; // Von Cup lösen
                ball.gameObject.SetActive(false); // Unsichtbar machen
            }
            
            // Alle Cups als falsch markieren
            foreach (Transform cup in cups)
            {
                cup.GetComponent<Cup>().isCorrectCup = false;
            }
            correctCup = null;
        }
    }
}


    public async Task MoveAllCupsDown(Transform[] cups, float amount, float duration)
    {
        foreach (Transform cup in cups)
        {
            await moveUpOrDown(cup, amount, duration);
        }
    }

    public async Task moveUpOrDown(Transform cup, float amount, float duration)
    {
        Vector3 startPos = cup.position;
        Vector3 endPos = startPos + new Vector3(0, amount, 0);

        float t = 0;
        while (t < 1)
        {
            t += Time.deltaTime / duration;
            cup.position = Vector3.Lerp(startPos, endPos, t);
            await Task.Yield();
        }
    }

    public Task<Cup> WaitForCupClick()
    {
        Debug.Log("Warte auf Becher-Auswahl...");
        clickTaskSource = new TaskCompletionSource<Cup>();
        return clickTaskSource.Task;
    }

    private void ResetCupPositions()
    {
        for (int i = 0; i < cups.Length; i++)
        {
            cups[i].position = startPositions[i];
            cups[i].GetComponent<Cup>().isCorrectCup = false;
        }
    }
async Task WaitForItemUsePhase()
{
    Debug.Log("Item-Phase gestartet! Klicke auf Items um sie zu benutzen...");

  
    float timer = 3f;
    while (timer > 0)
    {
        timer -= Time.deltaTime;
        await Task.Yield();
    }

    Debug.Log("Item-Phase beendet!");
}
}
