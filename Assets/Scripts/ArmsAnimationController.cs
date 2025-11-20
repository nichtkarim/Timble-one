using UnityEngine;

public class ArmsAnimationController : MonoBehaviour
{
    [Header("Weapon Bob Settings")]
    public float bobSpeed = 10f;
    public float bobAmount = 0.05f;
    public float sprintBobMultiplier = 1.5f;
    
    [Header("Weapon Sway Settings")]
    public float swayAmount = 0.02f;
    public float swaySmoothing = 6f;
    
    private Vector3 originalPosition;
    private CharacterController characterController;
    private float bobTimer = 0f;

    void Start()
    {
        originalPosition = transform.localPosition;
        
        // Character Controller des Eltern-Objekts finden
        characterController = GetComponentInParent<CharacterController>();
    }

    void Update()
    {
        HandleBobbing();
        HandleSway();
    }

    void HandleBobbing()
    {
        if (characterController == null) return;

        // Prüfen ob sich der Spieler bewegt
        Vector3 velocity = characterController.velocity;
        float horizontalVelocity = new Vector3(velocity.x, 0, velocity.z).magnitude;
        
        if (horizontalVelocity > 0.1f && characterController.isGrounded)
        {
            // Bob-Effekt berechnen
            float speedMultiplier = Input.GetKey(KeyCode.LeftShift) ? sprintBobMultiplier : 1f;
            bobTimer += Time.deltaTime * bobSpeed * speedMultiplier;
            
            float bobOffsetY = Mathf.Sin(bobTimer) * bobAmount;
            float bobOffsetX = Mathf.Cos(bobTimer * 0.5f) * bobAmount * 0.5f;
            
            Vector3 bobOffset = new Vector3(bobOffsetX, bobOffsetY, 0);
            transform.localPosition = originalPosition + bobOffset;
        }
        else
        {
            // Sanft zur Original-Position zurückkehren
            bobTimer = 0f;
            transform.localPosition = Vector3.Lerp(transform.localPosition, originalPosition, Time.deltaTime * 5f);
        }
    }

    void HandleSway()
    {
        // Maus-Bewegung für Sway
        float mouseX = Input.GetAxis("Mouse X") * swayAmount;
        float mouseY = Input.GetAxis("Mouse Y") * swayAmount;

        // Sway-Rotation berechnen (entgegengesetzt zur Mausbewegung)
        Quaternion rotationX = Quaternion.AngleAxis(-mouseY, Vector3.right);
        Quaternion rotationY = Quaternion.AngleAxis(mouseX, Vector3.up);
        Quaternion targetRotation = rotationX * rotationY;

        // Sanft zur Ziel-Rotation interpolieren
        transform.localRotation = Quaternion.Slerp(transform.localRotation, targetRotation, Time.deltaTime * swaySmoothing);
    }
}
