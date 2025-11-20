using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CustomHealthBar : MonoBehaviour
{
    [Header("Health Bar Images")]
    public Image backgroundBar; // Das leere/graue Bar
    public Image fillBar;       // Das rote/gefüllte Bar
    
    [Header("Health Text")]
    public TextMeshProUGUI healthText;
    
    [Header("Settings")]
    public HealthSystem targetHealth; // Wird von außen gesetzt
    public Color fullHealthColor = Color.red;
    public Color lowHealthColor = new Color(0.8f, 0, 0); // Dunkelrot
    
    void Start()
    {
        // Setze Fill Bar auf "Filled" Mode
        if (fillBar != null)
        {
            fillBar.type = Image.Type.Filled;
            fillBar.fillMethod = Image.FillMethod.Horizontal;
            fillBar.fillOrigin = (int)Image.OriginHorizontal.Left;
        }
    }

    void Update()
    {
        if (targetHealth == null) return;
        
        UpdateHealthBar();
    }

    private void UpdateHealthBar()
    {
        int currentHP = targetHealth.getCurrentHealth();
        int maxHP = targetHealth.getMaxHealth();
        float percentage = targetHealth.GetHealthPercentage();

        Debug.Log($"Health Update: {currentHP}/{maxHP} = {percentage:F2} ({percentage * 100:F0}%)");

        // Update Fill Amount (0.0 bis 1.0)
        if (fillBar != null)
        {
            fillBar.fillAmount = percentage;
        }

        // Update Text
        if (healthText != null)
        {
            healthText.text = $"{currentHP} / {maxHP}";
        }

        // Update Color
        if (fillBar != null)
        {
            fillBar.color = Color.Lerp(lowHealthColor, fullHealthColor, percentage);
        }
    }

    // Für manuelles Setzen des HealthSystems
    public void SetHealthSystem(HealthSystem health)
    {
        targetHealth = health;
        
        if (health != null)
        {
            Debug.Log($"HealthSystem gesetzt: {health.getCurrentHealth()}/{health.getMaxHealth()}");
        }
    }
}