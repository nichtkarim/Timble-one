using UnityEngine;
using UnityEngine.InputSystem;


public class InputHandler : MonoBehaviour
{
    private Camera cam;

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

        if (Mouse.current.leftButton.wasPressedThisFrame)
        {
            Ray ray = cam.ScreenPointToRay(Mouse.current.position.ReadValue());
            if (Physics.Raycast(ray, out RaycastHit hit))
            {
                // Versuche das IClickable Interface zu bekommen
                IClickable clickable = hit.collider.GetComponent<IClickable>();
                if (clickable != null)
                {
                    clickable.OnClick();
                }
            }
        }
    }
}
