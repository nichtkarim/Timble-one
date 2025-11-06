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
            IHoverable hoverable = hit.collider.GetComponent<IHoverable>();
            
            if (hoverable != null)
            {
                // If we're hovering over a new object
                if (hoverable != lastHovered)
                {
                    // Exit the last hovered object
                    if (lastHovered != null)
                    {
                        lastHovered.OnHoverExit();
                        Debug.Log("not hovering");
                    }
                    
                    // Enter the new hovered object
                    hoverable.OnHoverEnter();
                    Debug.Log("hovering");
                    lastHovered = hoverable;
                }
            }
            else
            {
                // Hit something but it's not hoverable
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
            // Not hitting anything
            if (lastHovered != null)
            {
                lastHovered.OnHoverExit();
                Debug.Log("not hovering");
                lastHovered = null;
            }
        }

        // Click Detection
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