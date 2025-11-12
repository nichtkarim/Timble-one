using UnityEngine;

[System.Serializable]
public class GameItem
{
    public string itemName;
    public GameObject itemPrefab; // 3D-Visualisierung
    public Sprite icon;
    public string description;

    public void OnHoverEnter()
    {
        throw new System.NotImplementedException();
    }

    public void OnHoverExit()
    {
        throw new System.NotImplementedException();
    }

    // Beispiel: Item-Effekt ausführen
    public virtual void Use()
    {
        Debug.Log($"{itemName} wurde benutzt!");
    }
}