using UnityEngine;
using System;

[System.Serializable]
public class GameItem 
{
    public string itemName;
    public GameObject itemPrefab; // 3D-Visualisierung
    public Sprite icon;
    public string description;


    // Beispiel: Item-Effekt ausführen
    public virtual void Use()
    {
        Debug.Log($"{itemName} wurde benutzt!");
    }
}
