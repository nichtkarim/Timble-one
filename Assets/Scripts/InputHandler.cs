using UnityEngine;
using UnityEngine.InputSystem;

public class InputHandler : MonoBehaviour
{
    private Camera cam;
    private IHoverable lastHovered;

    void Start()
    {
        cam = Camera.main;
        if (cam == null)
        {
            Debug.LogError("Keine Kamera mit Tag 'MainCamera' gefunden!");
        }
    }

    void Update()
    {
        if (cam == null) return;
        // Hover Detection
        Ray ray = cam.ScreenPointToRay(Mouse.current.position.ReadValue());
        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            // same as  IHoverable hoverable3 = hit.collider.gameObject.GetComponent<IHoverable>();
            IHoverable hoverable = hit.collider.GetComponent<IHoverable>();

            if (hoverable != null)
            {
                if (hoverable != lastHovered)
                {
                    if (lastHovered != null)
                    {
                        lastHovered.OnHoverExit();
                        Debug.Log("not hovering");
                    }
                    hoverable.OnHoverEnter();
                    Debug.Log("hovering");
                    lastHovered = hoverable;
                }
            }
            else
            {

                if (lastHovered != null)
                {
                    lastHovered.OnHoverExit();
                    Debug.Log("not hovering");
                    lastHovered = null;
                }
            }
        }
        else
        {

            if (lastHovered != null)
            {
                lastHovered.OnHoverExit();
                Debug.Log("not hovering");
                lastHovered = null;
            }
        }


        if (Mouse.current.leftButton.wasPressedThisFrame)
        {
            Ray clickRay = cam.ScreenPointToRay(Mouse.current.position.ReadValue());
            if (Physics.Raycast(clickRay, out RaycastHit clickHit))
            {
                IClickable clickable = clickHit.collider.GetComponent<IClickable>();
                if (clickable != null)
                {
                    clickable.OnClick();
                }
            }
        }
    }
}