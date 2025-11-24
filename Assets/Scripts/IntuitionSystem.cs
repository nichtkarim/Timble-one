using UnityEngine;
using TMPro;

public class IntuitionSystem : MonoBehaviour
{
    public static IntuitionSystem Instance;

    [Header("Intuition Settings")]
    [SerializeField] private int maxIntuition = 100;
    [SerializeField] private int currentIntuition; // Jetzt im Inspector sichtbar!
    
    [Header("Ball Removal")]
    public float ballRemovalBaseChance = 0.1f; // 10% Basis-Chance
    
    [Header("Tip System")]
    private int tipCupIndex = -1;
    private bool hasReceivedTip = false;
    
    [Header("Intuition Loss")]
    public float intuitionLossPerSecond = 1f; // 1% pro Sekunde
    public int intuitionLossOnWrongCup = 5; // 5% bei falscher Cup-Auswahl
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
    /// Entscheidet ob Ball entfernt wird BEVOR gemischt wird.
    /// Höhere Intuition = niedrigere Chance dass Ball entfernt wird
    /// </summary>
    public bool ShouldRemoveBall()
    {
        // Bei 100% Intuition: 0% Chance
        // Bei 0% Intuition: 10% Chance
        float intuitionFactor = 1f - GetIntuitionAsFloat();
        float removalChance = ballRemovalBaseChance * intuitionFactor;
        
        bool shouldRemove = Random.value < removalChance;
        
        Debug.Log($"🎲 Ball entfernen? {shouldRemove} (Chance: {removalChance * 100f:F1}%, Intuition: {currentIntuition}%)");
        
        return shouldRemove;
    }

    // === TIP SYSTEM (NACH DEM MISCHEN) ===
    
    /// <summary>
    /// Gibt dem Spieler einen Tipp basierend auf Intuition.
    /// Höhere Intuition = höhere Chance auf Tipp
    /// </summary>
    public void GiveTipToPlayer(int correctCupIndex)
    {
        tipCupIndex = -1;
        hasReceivedTip = false;
        
        float intuition = GetIntuitionAsFloat();
        
        // Bei 100% Intuition = 100% Tipp-Chance
        // Bei 30% Intuition = 30% Tipp-Chance
        if (Random.value < intuition)
        {
            tipCupIndex = correctCupIndex;
            hasReceivedTip = true;
            
            Debug.Log($"💡 Tipp erhalten! Richtige Tasse: {correctCupIndex + 1} (Intuition: {currentIntuition}%)");
            
            // Visuelles Feedback
            ShowTipEffect();
        }
        else
        {
            Debug.Log($"❌ Kein Tipp (Intuition: {currentIntuition}% war zu niedrig)");
        }
    }
    
    public bool HasTip()
    {
        return hasReceivedTip;
    }
    
    public int GetTipCupIndex()
    {
        return tipCupIndex;
    }
    
    private void ShowTipEffect()
    {
        // Flashscreen in Cyan = Tipp erhalten
        if (VisualFeedbackManager.Instance != null)
        {
            VisualFeedbackManager.Instance.FlashScreen(Color.cyan, 0.3f, 0.2f);
        }
    }

    // === BLUFF SYSTEM ===
    
    /// <summary>
    /// Spieler wählt Becher OHNE Tipp zu haben - zeigt "Bluff callen" Button
    /// </summary>
    public bool CanCallBluff()
    {
        return hasReceivedTip;
    }
    
    /// <summary>
    /// Spieler callt Bluff - Ball ist NICHT in der angegebenen Tasse
    /// </summary>
    public void CallBluff(int selectedCupIndex, bool ballWasInCup)
    {
        if (!hasReceivedTip)
        {
            Debug.LogWarning("⚠️ Kann nicht bluffen ohne Tipp!");
            return;
        }
        
        // Bluff ist richtig wenn Ball NICHT in der Tasse war
        bool bluffWasCorrect = !ballWasInCup;
        
        if (bluffWasCorrect)
        {
            Debug.Log("✅ BLUFF RICHTIG! Dealer hat keinen Ball in der Tasse → Dealer verliert Leben");
            // TODO: Dealer Leben abziehen
        }
        else
        {
            Debug.Log("❌ BLUFF FALSCH! Ball war doch in der Tasse → Spieler verliert Leben + Intuition");
            
            // Spieler verliert Leben
            if (MainGameLogic.Instance?.Player != null)
            {
                int hp = MainGameLogic.Instance.Player.getCurrentHealth();
                MainGameLogic.Instance.Player.setCurrentHealth(hp - 1);
            }
            
            // Große Intuition-Strafe
            setCurrentIntuition(intuitionLossOnWrongBluff);
        }
        
        ResetTip();
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
