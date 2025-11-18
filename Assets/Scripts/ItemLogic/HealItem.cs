using Unity.Mathematics;
using UnityEngine;

[System.Serializable]
public class HealItem : GameItem
{
    public string itemName;
    public Sprite icon;
    public string description;

    // Beispiel: Item-Effekt ausführen
    
    public override void Use()
    {
      
        
        // Heile den Spieler
        if (MainGameLogic.Instance != null && MainGameLogic.Instance.Player != null)
        {
            int healthBefore = MainGameLogic.Instance.Player.getCurrentHealth();
            if(healthBefore >= MainGameLogic.Instance.Player.getMaxHealth())
            {
                Debug.Log("Gesundheit ist bereits voll. Heilung nicht möglich.");
                return;
            }
    
            
            MainGameLogic.Instance.Player.setCurrentHealth(healthBefore + 1);
            
            // Visuelles Feedback für Heilung
            if (VisualFeedbackManager.Instance != null && Camera.main != null)
            {
                Vector3 feedbackPos = Camera.main.transform.position + Camera.main.transform.forward * 2f;
                VisualFeedbackManager.Instance.PlayHealEffect(feedbackPos);
                FloatingText.CreateHealText(1, feedbackPos);
            }
        }
        else
        {
            Debug.LogError("MainGameLogic oder Player nicht gefunden!");
        }
    }
}

