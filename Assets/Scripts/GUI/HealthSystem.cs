using UnityEngine;

public class HealthSystem
{
    int maxHealth = 5;
    int currentHealth;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public HealthSystem()
    {
        currentHealth = maxHealth;
    }
    public void takeDamage(int damage)
    {
        currentHealth -= damage;
        Debug.Log("Player/Dealer took " + damage + " damage. Current health: " + currentHealth);
        
        // Visuelles Feedback für Schaden
        if (VisualFeedbackManager.Instance != null && MainGameLogic.Instance != null)
        {
            // Finde Position des Spielers (falls vorhanden)
            VisualFeedbackManager.Instance.ShakeCamera(0.3f, 0.15f);
        }
        
        if (currentHealth <= 0)
        {
            Debug.Log("Player/Dealer is dead!");
        }
    }
    public int getCurrentHealth()
    {
        return currentHealth;
    }
    public void setCurrentHealth(int health)
    {
        currentHealth = health;
    }
    public int getMaxHealth()
    {
        return maxHealth;
    }
    public float GetHealthPercentage()
    {
        return (float)currentHealth / maxHealth;
    }
}
