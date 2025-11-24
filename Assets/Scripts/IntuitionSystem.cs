using UnityEngine;
using TMPro;

public class IntuitionSystem : MonoBehaviour
{
    public static IntuitionSystem Instance;

    [Header("Intuition Settings")]
    [SerializeField] private int maxIntuition = 100;
    [SerializeField] private int currentIntuition; // Jetzt im Inspector sichtbar!
    
    [Header("Ball Removal")]
    public float ballRemovalChance = 0.25f; // 25% Chance dass Dealer schummelt
    
    [Header("Tip System")]
    private int tipCupIndex = -1;
    private bool hasReceivedTip = false;
    private bool dealerCheatedThisRound = false; // Wurde Ball entfernt?
    
    [Header("Intuition Loss")]
    public float intuitionLossPerSecond = 1f; // 1% pro Sekunde
    public int intuitionLossOnWrongCup = 5; // 3% bei falscher Cup-Auswahl
    public int intuitionLossOnWrongBluff = 20; // Bei falschem Bluff

    [Header("UI References")]
    public TextMeshProUGUI intuitionText;
    public GameObject intuitionPanel;

    private float intuitionFloat; // Für präzisen Verlust über Zeit

    void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
            
        currentIntuition = maxIntuition;
        intuitionFloat = maxIntuition; // Initialisiere Float-Version
    }

    void Start()
    {
        UpdateIntuitionUI();
    }

    void Update()
    {
        // Kontinuierlicher Intuition-Verlust über Zeit (1% pro Sekunde)
        if (intuitionFloat > 0)
        {
            // Ziehe Zeit-Verlust vom Float ab (präzise)
            intuitionFloat -= intuitionLossPerSecond * Time.deltaTime;
            intuitionFloat = Mathf.Max(0, intuitionFloat);
            
            // Konvertiere zu Int für Anzeige
            int newIntuition = Mathf.FloorToInt(intuitionFloat);
            
            // Update nur wenn sich der Int-Wert ändert
            if (newIntuition != currentIntuition)
            {
                currentIntuition = newIntuition;
                UpdateIntuitionUI();
                
                // Debug alle 10%
                if (currentIntuition % 10 == 0)
                {
                    Debug.Log($"⏱️ Intuition durch Zeit-Verlust: {currentIntuition}%");
                }
            }
        }
    }

    // === INTUITION MANAGEMENT ===
    
    public void IncreaseIntuition(int amount)
    {
        intuitionFloat = Mathf.Clamp(intuitionFloat + amount, 0, maxIntuition);
        currentIntuition = Mathf.FloorToInt(intuitionFloat);
        Debug.Log($"✨ Intuition erhöht auf {currentIntuition}%");
        UpdateIntuitionUI();
    }
    
    public void setCurrentIntuition(int damage)
    {
        if (intuitionFloat > 0)
        {
            intuitionFloat -= damage;
            intuitionFloat = Mathf.Max(0, intuitionFloat);
            currentIntuition = Mathf.FloorToInt(intuitionFloat);
            Debug.Log($"📉 Intuition verringert auf {currentIntuition}%");
            UpdateIntuitionUI();
        }
    }
    
    public int getCurrentIntuition()
    {
        return currentIntuition;
    }
    
    public int getMaxIntuition()
    {
        return maxIntuition;
    }
    
    public float GetIntuitionAsFloat()
    {
        return (float)currentIntuition / maxIntuition; // 0.0 bis 1.0
    }

    // === BALL REMOVAL (VOR DEM MISCHEN) ===
    
    /// <summary>
    /// Entscheidet ob Dealer schummelt und Ball entfernt (25% Chance)
    /// UNABHÄNGIG von Intuition!
    /// </summary>
    public bool ShouldRemoveBall()
    {
        // Feste 25% Chance
        dealerCheatedThisRound = Random.value < ballRemovalChance;
        
        Debug.Log($"🎲 Dealer schummelt? {dealerCheatedThisRound} (Chance: {ballRemovalChance * 100f}%)");
        
        return dealerCheatedThisRound;
    }

    // === TIP SYSTEM (NACH DEM MISCHEN) ===
    
    /// <summary>
    /// Gibt dem Spieler einen Tipp OB DEALER GESCHUMMELT HAT.
    /// Höhere Intuition = höhere Chance den Betrug zu erkennen
    /// </summary>
    public void GiveCheatingTip()
    {
        hasReceivedTip = false;
        
        // Nur Tipp geben wenn Dealer TATSÄCHLICH geschummelt hat
        if (!dealerCheatedThisRound)
        {
            Debug.Log("✅ Dealer hat NICHT geschummelt - kein Tipp nötig");
            return;
        }
        
        float intuition = GetIntuitionAsFloat();
        
        // Bei 100% Intuition = 100% Tipp-Chance
        // Bei 30% Intuition = 30% Tipp-Chance
        if (Random.value < intuition)
        {
            hasReceivedTip = true;
            
            Debug.Log($"💡 TIPP ERHALTEN! Dealer hat geschummelt! (Intuition: {currentIntuition}%)");
            
            // Visuelles Feedback - Cyan Flash
            ShowCheatingTipEffect();
        }
        else
        {
            Debug.Log($"❌ Kein Tipp (Intuition: {currentIntuition}% war zu niedrig - Dealer hat geschummelt aber Spieler merkt es nicht)");
        }
    }
    
    /// <summary>
    /// Prüft ob Spieler einen Tipp über Betrug bekommen hat
    /// </summary>
    public bool HasCheatingTip()
    {
        return hasReceivedTip;
    }
    
    /// <summary>
    /// Wurde der Ball diese Runde entfernt?
    /// </summary>
    public bool DealerCheatedThisRound()
    {
        return dealerCheatedThisRound;
    }
    
    private void ShowCheatingTipEffect()
    {
        // Cyan Flashscreen = Dealer hat geschummelt!
        if (VisualFeedbackManager.Instance != null)
        {
            VisualFeedbackManager.Instance.FlashScreen(Color.cyan, 0.5f, 0.3f);
        }
    }

    // === BLUFF SYSTEM ===
    
    /// <summary>
    /// Spieler callt Bluff - behauptet Dealer hat geschummelt
    /// </summary>
    public void CallBluff(out bool bluffWasCorrect)
    {
        bluffWasCorrect = dealerCheatedThisRound;
        
        if (bluffWasCorrect)
        {
            Debug.Log("✅ BLUFF RICHTIG! Dealer HAT geschummelt!");
        }
        else
        {
            Debug.Log("❌ BLUFF FALSCH! Dealer hat NICHT geschummelt - Ball war im Spiel!");
            
            // Intuition-Strafe nur bei falschem Bluff
            setCurrentIntuition(intuitionLossOnWrongBluff);
        }
    }

    // === ROUND MANAGEMENT ===
    
    /// <summary>
    /// Wird aufgerufen wenn Spieler FALSCHEN Cup wählt - Extra Strafe
    /// </summary>
    public void OnWrongCupSelected()
    {
        setCurrentIntuition(intuitionLossOnWrongCup);
        Debug.Log($"❌ Falscher Cup gewählt! Intuition -{intuitionLossOnWrongCup}% → {currentIntuition}%");
    }
    
    /// <summary>
    /// Wird am Ende jeder Runde aufgerufen - Reset Tipp
    /// </summary>
    public void OnRoundEnd()
    {
        ResetTip();
        dealerCheatedThisRound = false; // Reset für nächste Runde
        // Kein Extra-Verlust mehr hier - läuft kontinuierlich über Update()
    }
    
    private void ResetTip()
    {
        hasReceivedTip = false;
        tipCupIndex = -1;
    }

    // === UI UPDATE ===
    
    /// <summary>
    /// Aktualisiert die UI-Anzeige der Intuition
    /// </summary>
    private void UpdateIntuitionUI()
    {
        if (intuitionText != null)
        {
            intuitionText.text = $"Intuition: {currentIntuition}%";
            
            // Farbe basierend auf Intuition
            if (currentIntuition >= 70)
                intuitionText.color = Color.green;
            else if (currentIntuition >= 30)
                intuitionText.color = Color.yellow;
            else
                intuitionText.color = Color.red;
        }
    }
}
