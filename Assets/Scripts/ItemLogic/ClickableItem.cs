using UnityEngine;

/// <summary>
/// Diese Komponente wird dynamisch zu gespawnten Item-Prefabs hinzugefügt
/// und leitet Clicks/Hovers an den parent Slot weiter
/// </summary>
public class ClickableItem : MonoBehaviour, IClickable, IHoverable
{
    [HideInInspector]
    public ItemSlot parentSlot;

    public void OnClick()
    {
        if (!MainGameLogic.PlayerCanClick)
        {
            Debug.Log("Spieler kann momentan nicht klicken");
            return;
        }

        if (parentSlot != null)
        {
            Debug.Log($"Item wurde geklickt - leite an Slot {parentSlot.slotIndex} weiter");
            parentSlot.OnClick();
        }
    }

    public void OnHoverEnter()
    {
        if (!MainGameLogic.PlayerCanClick)
        {
            return;
        }

        // Füge Outline-Effekt hinzu
        if (gameObject.GetComponent<Outline>() == null)
        {
            var outline = gameObject.AddComponent<Outline>();
            outline.OutlineMode = Outline.Mode.OutlineAll;
            outline.OutlineColor = Color.yellow; // Gelb für Items
            outline.OutlineWidth = 5f;
            
            Debug.Log($"Hover Enter auf Item in Slot {parentSlot?.slotIndex}");
        }
    }

    public void OnHoverExit()
    {
        if (!MainGameLogic.PlayerCanClick)
        {
            return;
        }

        // Entferne Outline-Effekt
        var outline = gameObject.GetComponent<Outline>();
        if (outline != null)
        {
            Destroy(outline);
            Debug.Log($"Hover Exit auf Item in Slot {parentSlot?.slotIndex}");
        }
    }
}