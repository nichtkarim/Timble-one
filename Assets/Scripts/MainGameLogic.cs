using UnityEngine;
using System.Collections;
using System.Linq;
using Unity.VisualScripting;
using System.Threading.Tasks;


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
    
    
    void Awake()
    {
        Instance = this;
        startPositions = cups.Select(c => c.position).ToArray();
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
        // 1. Zufällig Cup bestimmen
        correctCup = cups[Random.Range(0, cups.Length)];
        correctCup.GetComponent<Cup>().isCorrectCup = true;

        ball.position = new Vector3(correctCup.position.x, ball.position.y, ball.position.z);

        await MoveAllCupsDown(cups, -0.4f, 0.7f);

        ball.parent = correctCup;
      
        // === SCHRITT 3: BALL ENTFERNUNG (VOR dem Mischen) ===
        bool ballWasRemoved = false;
        if (IntuitionSystem.Instance != null)
        {
            ballWasRemoved = IntuitionSystem.Instance.ShouldRemoveBall();
            
            if (ballWasRemoved)
            {
                Debug.Log("⚠️ DEALER HAT DEN BALL ENTFERNT! (Niedrige Intuition)");
                // Deaktiviere den Ball visuell
                ball.gameObject.SetActive(false);
                
                // Wichtig: Markiere dass KEIN Cup korrekt ist
                correctCup.GetComponent<Cup>().isCorrectCup = false;
                correctCup = null; // Kein korrekter Cup mehr!
            }
            else
            {
                Debug.Log("✅ Ball bleibt im Spiel (Hohe Intuition schützt)");
            }
        }
        
        PlayerCanClick = false;
        await shuffleCups();
        
        // === SCHRITT 4: TIPP-SYSTEM (NACH dem Mischen) ===
        if (IntuitionSystem.Instance != null && !ballWasRemoved)
        {
            // Nur Tipp geben wenn Ball noch im Spiel ist
            int correctCupIndex = System.Array.IndexOf(cups, correctCup);
            IntuitionSystem.Instance.GiveTipToPlayer(correctCupIndex);
            
            if (IntuitionSystem.Instance.HasTip())
            {
                int tipIndex = IntuitionSystem.Instance.GetTipCupIndex();
                Debug.Log($"💡 SPIELER HAT TIPP ERHALTEN! Richtige Tasse: {cups[tipIndex].name} (Index {tipIndex})");
                
                // Optional: Visueller Effekt für den Tipp
                // TODO: Hier könnte man den richtigen Cup kurz highlighten
            }
            else
            {
                Debug.Log($"❌ Kein Tipp erhalten (Intuition zu niedrig: {IntuitionSystem.Instance.getCurrentIntuition()}%)");
            }
        }
        
        PlayerCanClick = true; 
        ball.parent = null;

        Cup chosenCup = await WaitForCupClick();
        
        // === Ball wieder aktivieren falls er entfernt wurde ===
        if (ballWasRemoved)
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
