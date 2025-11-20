using UnityEngine;

public class ItemSlot : MonoBehaviour, IClickable, IHoverable
{
    public GameItem currentItem;
    private GameObject visualInstance;
    public int slotIndex;

    public bool HasItem => currentItem != null;

    void Start()
    {
        // Stelle sicher, dass ein Collider vorhanden ist für Raycasts
        if (GetComponent<Collider>() == null)
        {
            BoxCollider collider = gameObject.AddComponent<BoxCollider>();
            collider.size = new Vector3(1f, 1f, 0.1f);
            Debug.Log($"Collider zu Slot {slotIndex} hinzugefügt");
        }
    }

    public void SetItem(GameItem newItem)
    {
        ClearSlot();
        currentItem = newItem;

        if (currentItem != null && currentItem.itemPrefab != null)
        {
            Debug.Log($"Spawne {currentItem.itemName} in Slot {slotIndex}");
            visualInstance = Instantiate(
                currentItem.itemPrefab, 
                transform.position, 
                Quaternion.identity, 
                transform
            );
            
            // Zentriere das Item im Slot
            visualInstance.transform.localPosition = Vector3.zero;
            
            // Skaliere das Item basierend auf der Slot-Größe
            ScaleItemToFitSlot();
        }
        else
        {
            Debug.LogError($"Item oder Prefab ist null für Slot {slotIndex}");
        }
    }
    
    private void ScaleItemToFitSlot()
    {
        if (visualInstance == null) return;
        
        // Hole den Collider des Slots
        Collider slotCollider = GetComponent<Collider>();
        if (slotCollider == null)
        {
            Debug.LogWarning($"Slot {slotIndex} hat keinen Collider! Verwende Standard-Skalierung.");
            visualInstance.transform.localScale = Vector3.one * 0.1f;
            return;
        }
        
        // Berechne die Bounds des Items (alle Renderer im Item)
        Renderer[] renderers = visualInstance.GetComponentsInChildren<Renderer>();
        if (renderers.Length == 0)
        {
            Debug.LogWarning($"Item {currentItem.itemName} hat keine Renderer!");
            return;
        }
        
        // Berechne die Gesamt-Bounds des Items
        Bounds itemBounds = renderers[0].bounds;
        foreach (Renderer renderer in renderers)
        {
            itemBounds.Encapsulate(renderer.bounds);
        }

        // Größe des Slots (BoxCollider)
        Vector3 slotSize = slotCollider.bounds.size;
        
        // Optional: Automatische Skalierung (auskommentiert)
        // Vector3 itemSize = itemBounds.size;
        // float scaleX = slotSize.x / itemSize.x;
        // float scaleY = slotSize.y / itemSize.y;
        // float scaleZ = slotSize.z / itemSize.z;
        // float scaleFactor = Mathf.Min(scaleX, scaleY, scaleZ) * 0.8f;
        // visualInstance.transform.localScale = Vector3.one * scaleFactor;
    }

    public void ClearSlot()
    {
        if (visualInstance != null)
        {
            Debug.Log($"Lösche Item aus Slot {slotIndex}");
            
            // Entferne Outline BEVOR das Objekt zerstört wird
            var outline = visualInstance.GetComponent<Outline>();
            if (outline != null)
            {
                Destroy(outline);
            }
            
            Destroy(visualInstance);
        }

        currentItem = null;
    }

    public void OnClick()
    {
        if (!MainGameLogic.PlayerCanClick) return;
        if (currentItem == null) return;

        Debug.Log($"Item {currentItem.itemName} in Slot {slotIndex} benutzt.");
        currentItem.Use();
        ClearSlot();

        InventorySystem.Instance.OnItemUsed();
    }

    public void OnHoverEnter()
    {
        if (!MainGameLogic.PlayerCanClick) return;
        if (currentItem == null) return;

        // Zeige Tooltip
        if (TooltipSystem.Instance != null)
        {
            TooltipSystem.Instance.ShowTooltip(currentItem.itemName, currentItem.description);
        }

        // Füge Outline zum Slot hinzu
        if (gameObject.GetComponent<Outline>() == null)
        {
            var outline = gameObject.AddComponent<Outline>();
            outline.OutlineMode = Outline.Mode.OutlineAll;
            outline.OutlineColor = Color.yellow;
            outline.OutlineWidth = 4f;
        }
    }

    public void OnHoverExit()
    {
        if (!MainGameLogic.PlayerCanClick) return;

        // Verstecke Tooltip
        if (TooltipSystem.Instance != null)
        {
            TooltipSystem.Instance.HideTooltip();
        }

        // Entferne Outline vom Slot
        var outline = gameObject.GetComponent<Outline>();
        if (outline != null)
        {
            Destroy(outline);
        }
    }
}