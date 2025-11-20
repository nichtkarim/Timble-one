using UnityEngine;
using System.Collections.Generic;

public class InventorySystem : MonoBehaviour
{
    public static InventorySystem Instance;

    public ItemSlot[] slots; // 4 Slot GameObjects in der Szene
    public GameObject healItemPrefab;
    public GameObject slowDownItemPrefab;
    public GameObject IntuitionItemPrefab;



    private bool isInitialized = false;

    void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
            
        // Initialisiere die Slots hier in Awake
        InitializeSlots();
    }

    private void InitializeSlots()
    {
        if (slots == null || slots.Length == 0)
        {
            Debug.LogError("Keine Slots zugewiesen! Bitte im Inspector zuweisen.");
            return;
        }

        Debug.Log($"Initialisiere {slots.Length} Slots");
        
        // Stelle sicher, dass alle Slots leer sind
        foreach (var slot in slots)
        {
            if (slot != null)
            {
                slot.ClearSlot();
            }
        }
        
        isInitialized = true;
    }

    public void RefillSlots()
    {
        if (!isInitialized)
        {
            Debug.LogWarning("InventorySystem noch nicht initialisiert!");
            InitializeSlots();
        }

        Debug.Log("RefillSlots wurde aufgerufen");
        
        if (slots == null || slots.Length == 0)
        {
            Debug.LogError("Keine Slots zugewiesen!");
            return;
        }

        foreach (var slot in slots)
        {
            if (slot == null)
            {
                Debug.LogError("Ein Slot ist null!");
                continue;
            }

            Debug.Log($"Prüfe Slot {slot.slotIndex}");
            
            if (!slot.HasItem)
            {
                Debug.Log($"Slot {slot.slotIndex} ist leer - füge Item hinzu");
                GameItem newItem = GenerateRandomItem();
                if (newItem != null)
                {
                    slot.SetItem(newItem);
                }
            }
            else
            {
                Debug.Log($"Slot {slot.slotIndex} hat bereits ein Item: {slot.currentItem.itemName}");
            }
        }
    }

    private GameItem GenerateRandomItem()
    {
        if (healItemPrefab == null || slowDownItemPrefab == null)
        {
            Debug.LogError("Item Prefabs sind nicht zugewiesen im Inspector!");
            return null;
        }

        int r = Random.Range(0, 3);
        if (r == 0)
        {
            Debug.Log("Generiere Heiltrank");
            HealItem x = new HealItem();
            x.itemName = "Heiltrank";
            x.itemPrefab = healItemPrefab;
            x.description = "Heilt um 1 Lebenspunkt";
            
            return x;
        }
        else if( r == 1)
        {
            Debug.Log("Generiere regen Intuition Item");
            return new IntuitionItem 
            { 
                itemName = "IntuitionsItem", 
                itemPrefab = IntuitionItemPrefab,
                description = "Erhöht die Intuition für die nächste Runde",
            };
        }
        else
        {
            Debug.Log("Generiere VerlangsamungsItems");
            return new SlowDownItem 
            { 
                itemName = "VerlangsamungsItem", 
                itemPrefab = slowDownItemPrefab,
                description = "Verlangsamt um 30 Prozent für die nächste Runde",
            };
        }
    }

    public void OnItemUsed()
    {
        Debug.Log("Item wurde verbraucht.");
    }
}