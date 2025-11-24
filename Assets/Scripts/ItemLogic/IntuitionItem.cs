using Unity.Mathematics;
using UnityEngine;

[System.Serializable]
public class IntuitionItem : GameItem
{
    // itemName, icon, description werden von GameItem geerbt - NICHT hier definieren!

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
        }
        else
        {
            Debug.LogError("MainGameLogic oder Player nicht gefunden!");
        }
    }
}

