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
        if (currentHealth <= 0)
        {
            Debug.Log("Player/Dealer is dead!");
        }
    }
    public int getCurrentHealth()
    {
        return currentHealth;
    }
}
