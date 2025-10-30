using UnityEngine;

using UnityEngine.InputSystem; // <--- neues Input System

public class InputHandler : MonoBehaviour
{
    void Update()
    {
        if (Mouse.current.leftButton.wasPressedThisFrame)
        {
            Ray ray = Camera.main.ScreenPointToRay(Mouse.current.position.ReadValue());
            if (Physics.Raycast(ray, out RaycastHit hit))
            {
                if (hit.collider.CompareTag("Clickable"))
                {
                    Debug.Log(hit.collider.name + " ausgewaehlt");
                }
            }
        }
    }
}
