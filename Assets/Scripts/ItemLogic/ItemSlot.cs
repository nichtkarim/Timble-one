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
        
        // Größe des Items
        /*
       / Vector3 itemSize = itemBounds.size;
        
        // Berechne Skalierungsfaktoren für jede Achse
        float scaleX = slotSize.x / itemSize.x;
        float scaleY = slotSize.y / itemSize.y;
        float scaleZ = slotSize.z / itemSize.z;
        
        // Nimm den kleinsten Faktor, damit das Item in alle Richtungen passt
        float scaleFactor = Mathf.Min(scaleX, scaleY, scaleZ);
        
        // Optional: Lass etwas Luft (80% der maximalen Größe)
        scaleFactor *= 0.8f;
        
        // Wende die Skalierung an
        visualInstance.transform.localScale = Vector3.one * scaleFactor;
        */
        //Debug.Log($"Item {currentItem.itemName} skaliert mit Faktor {scaleFactor}");
    }

    public void ClearSlot()
    {
        if (visualInstance != null)
        {
            Debug.Log($"Lösche Item aus Slot {slotIndex}");
            Destroy(visualInstance);
        }

        currentItem = null;
    }

    public void OnClick()
    {
        if (!MainGameLogic.PlayerCanClick)
        {
            Debug.Log("Spieler kann momentan nicht klicken");
            return;
        }
        
        if (currentItem == null)
        {
            Debug.Log($"Slot {slotIndex} ist leer");
            return;
        }

        Debug.Log($"Item {currentItem.itemName} in Slot {slotIndex} benutzt.");
        currentItem.Use();
        ClearSlot();

        InventorySystem.Instance.OnItemUsed();
    }

    public void OnHoverEnter()
    {
         if (!MainGameLogic.PlayerCanClick)
        {
            return;
        }
        else
        {
            if (gameObject.GetComponent<Outline>() == null)
            {
                 var outline = gameObject.AddComponent<Outline>();
            outline.OutlineMode = Outline.Mode.OutlineAll;
            outline.OutlineColor = Color.white;
            outline.OutlineWidth = 4f;
            }
            else
            {
                return;
            }
            
        }
     
    }

    public void OnHoverExit()
    {
        if (!MainGameLogic.PlayerCanClick)
        {
            return;
        }
        else
        {
            if (gameObject.GetComponent<Outline>() != null)
            {
                Destroy(gameObject.GetComponent<Outline>());
            }
        }
    }
}