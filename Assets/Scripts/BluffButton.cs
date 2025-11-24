using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// Verwaltet den Bluff-Button und seine Interaktion
/// </summary>
public class BluffButton : MonoBehaviour
{
    public static BluffButton Instance;

    [Header("UI References")]
    public Button bluffButton;
    public TextMeshProUGUI buttonText;
    public GameObject buttonPanel;

    [Header("Tip Info UI")]
    public GameObject tipInfoPanel;
    public TextMeshProUGUI tipInfoText;

    void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);

        // Button-Click Event
        if (bluffButton != null)
        {
            bluffButton.onClick.AddListener(OnBluffButtonClicked);
        }

        // Verstecke am Start
        HideBluffButton();
        HideTipInfo();
    }

    /// <summary>
    /// Zeigt Bluff-Button wenn Spieler einen Tipp hat
    /// </summary>
    public void ShowBluffButton()
    {
        if (buttonPanel != null)
        {
            buttonPanel.SetActive(true);
            Debug.Log("🃏 Bluff-Button angezeigt");
        }
    }

    /// <summary>
    /// Versteckt Bluff-Button
    /// </summary>
    public void HideBluffButton()
    {
        if (buttonPanel != null)
        {
            buttonPanel.SetActive(false);
        }
    }

    /// <summary>
    /// Zeigt Tipp-Info UI - Dealer hat geschummelt!
    /// </summary>
    public void ShowTipInfo()
    {
        if (tipInfoPanel != null && tipInfoText != null)
        {
            tipInfoPanel.SetActive(true);
            tipInfoText.text = "💡 Deine Intuition sagt: Dealer hat geschummelt!";
            Debug.Log("💡 Tipp-Info angezeigt: Dealer hat geschummelt");
        }
    }

    /// <summary>
    /// Versteckt Tipp-Info UI
    /// </summary>
    public void HideTipInfo()
    {
        if (tipInfoPanel != null)
        {
            tipInfoPanel.SetActive(false);
        }
    }

    /// <summary>
    /// Wird aufgerufen wenn Bluff-Button geklickt wird
    /// </summary>
    private void OnBluffButtonClicked()
    {
        Debug.Log("🃏 Bluff-Button geklickt!");

        // Informiere MainGameLogic dass Spieler blufft
        if (MainGameLogic.Instance != null)
        {
            MainGameLogic.Instance.OnBluffCalled();
        }

        // Verstecke Button
        HideBluffButton();
    }

    /// <summary>
    /// Aktiviert/Deaktiviert Button basierend auf Zustand
    /// </summary>
    public void SetButtonInteractable(bool interactable)
    {
        if (bluffButton != null)
        {
            bluffButton.interactable = interactable;
        }
    }
}
