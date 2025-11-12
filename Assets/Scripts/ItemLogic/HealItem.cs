using UnityEngine;

[System.Serializable]
public class HealItem : GameItem, IHoverable
{
    public string itemName;
    public Sprite icon;
    public string description;

    // Beispiel: Item-Effekt ausführen
    public virtual void Use()
    {
        Debug.Log($"{itemName} wurde benutzt!");
    }

    
}
