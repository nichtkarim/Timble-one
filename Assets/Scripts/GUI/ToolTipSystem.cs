using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.InputSystem;

public class TooltipSystem : MonoBehaviour
{
    public static TooltipSystem Instance;

    [Header("Tooltip UI")]
    public GameObject tooltipPanel;
    public TextMeshProUGUI itemNameText;
    public TextMeshProUGUI descriptionText;
    
    [Header("Settings")]
    public Vector2 offset = new Vector2(10, 10); // Abstand vom Mauszeiger
    
    private RectTransform tooltipRect;
    private Canvas canvas;

    void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);

        if (tooltipPanel != null)
        {
            tooltipRect = tooltipPanel.GetComponent<RectTransform>();
        }
        canvas = GetComponentInParent<Canvas>();
        
        // Tooltip am Start verstecken
        HideTooltip();
    }

    void Update()
    {
        if (tooltipPanel != null && tooltipPanel.activeSelf)
        {
            // Tooltip folgt Mauszeiger
            UpdateTooltipPosition();
        }
    }

    public void ShowTooltip(string itemName, string description)
    {
        if (tooltipPanel == null)
        {
            Debug.LogError("TooltipPanel ist null! Bitte im Inspector zuweisen.");
            return;
        }

        Debug.Log($"Zeige Tooltip: {itemName} - {description}");

        tooltipPanel.SetActive(true);
        
        if (itemNameText != null)
        {
            itemNameText.text = itemName;
            Debug.Log($"ItemNameText gesetzt auf: {itemName}");
        }
        else
        {
            Debug.LogError("ItemNameText ist null! Bitte im Inspector zuweisen.");
        }
        
        if (descriptionText != null)
        {
            descriptionText.text = description;
            Debug.Log($"DescriptionText gesetzt auf: {description}");
        }
        else
        {
            Debug.LogError("DescriptionText ist null! Bitte im Inspector zuweisen.");
        }
        
        UpdateTooltipPosition();
    }

    public void HideTooltip()
    {
        if (tooltipPanel != null)
        {
            tooltipPanel.SetActive(false);
        }
    }

    private void UpdateTooltipPosition()
    {
        // Verwende neues Input System für Mausposition
        Vector2 mousePosition = Mouse.current.position.ReadValue();
        
        // Konvertiere Mausposition zu Canvas-Koordinaten
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            canvas.transform as RectTransform,
            mousePosition,
            canvas.worldCamera,
            out Vector2 localPoint
        );

        // Setze Position mit Offset
        tooltipRect.localPosition = localPoint + offset;

        // Verhindere, dass Tooltip aus dem Screen geht
        ClampToScreen();
    }

    private void ClampToScreen()
    {
        Vector3[] corners = new Vector3[4];
        tooltipRect.GetWorldCorners(corners);

        RectTransform canvasRect = canvas.transform as RectTransform;
        
        // Prüfe ob Tooltip rechts aus dem Screen geht
        if (corners[2].x > Screen.width)
        {
            float diff = corners[2].x - Screen.width;
            tooltipRect.localPosition -= new Vector3(diff, 0, 0);
        }
        
        // Prüfe ob Tooltip oben aus dem Screen geht
        if (corners[1].y > Screen.height)
        {
            float diff = corners[1].y - Screen.height;
            tooltipRect.localPosition -= new Vector3(0, diff, 0);
        }
        
        // Prüfe ob Tooltip links aus dem Screen geht
        if (corners[0].x < 0)
        {
            tooltipRect.localPosition -= new Vector3(corners[0].x, 0, 0);
        }
        
        // Prüfe ob Tooltip unten aus dem Screen geht
        if (corners[0].y < 0)
        {
            tooltipRect.localPosition -= new Vector3(0, corners[0].y, 0);
        }
    }
}