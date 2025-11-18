using Unity.Mathematics;
using UnityEngine;

[System.Serializable]
public class SlowDownItem : GameItem
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
    
    
            
            MainGameLogic.Instance.roundBasedModifier = 0.1f;
        }
        else
        {
            Debug.LogError("MainGameLogic oder Player nicht gefunden!");
        }
    }
}

