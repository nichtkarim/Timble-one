using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(CharacterController))]
public class FirstPersonController : MonoBehaviour
{
    [Header("Movement Settings")]
    public float walkSpeed = 5f;
    public float sprintSpeed = 8f;
    public float crouchSpeed = 2.5f;
    public float jumpHeight = 1.5f;
    public float gravity = -9.81f;
    
    [Header("Head Bob Settings")]
    public bool enableHeadBob = true;
    public float bobSpeed = 14f;
    public float bobAmount = 0.05f;
    public float sprintBobMultiplier = 1.5f;

    [Header("Look Settings")]
    public float lookSensitivity = 2f;
    public float maxLookAngle = 89f; // Verhindert Überdrehung
    
    [Header("References")]
    public Transform cameraTransform;
    public Transform armsTransform; // Referenz zu den Armen

    private CharacterController characterController;
    private Vector3 velocity;
    private bool isGrounded;
    private float cameraPitch = 0f;
    
    private Vector2 moveInput;
    private Vector2 lookInput;
    private bool sprintInput;
    private bool jumpInput;
    private bool crouchInput;
    
    // Head Bob
    private float bobTimer = 0f;
    private Vector3 originalCameraPosition;

    void Awake()
    {
        characterController = GetComponent<CharacterController>();
        
        // Cursor verstecken und im Spiel sperren
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        
        Debug.Log("=== FIRSTPERSON CONTROLLER START ===");
        Debug.Log("CameraTransform zugewiesen? " + (cameraTransform != null));
        
        // Speichere Original-Position der Kamera und setze Rotation zurück
        if (cameraTransform != null)
        {
            originalCameraPosition = cameraTransform.localPosition;
            // Setze Kamera-Rotation auf 0 beim Start
            cameraTransform.localRotation = Quaternion.identity;
            cameraPitch = 0f;
            Debug.Log("✅ Kamera gefunden: " + cameraTransform.name);
            Debug.Log("Kamera Position: " + cameraTransform.localPosition);
            Debug.Log("Kamera Parent: " + (cameraTransform.parent != null ? cameraTransform.parent.name : "KEIN PARENT"));
        }
        else
        {
            Debug.LogError("❌❌❌ CAMERA TRANSFORM IST NULL! Bitte im Inspector zuweisen! ❌❌❌");
        }
    }

    void Update()
    {
        HandleMovement();
        HandleLook();
        HandleHeadBob();
        
        // ESC-Taste zum Entsperren des Cursors
        if (Keyboard.current.escapeKey.wasPressedThisFrame)
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
        
        // Linke Maustaste zum Sperren des Cursors
        if (Mouse.current.leftButton.wasPressedThisFrame && Cursor.lockState == CursorLockMode.None)
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
    }

    void HandleMovement()
    {
        // Ground Check
        isGrounded = characterController.isGrounded;
        if (isGrounded && velocity.y < 0)
        {
            velocity.y = -2f;
        }

        // Input lesen
        moveInput = Vector2.zero;
        if (Keyboard.current.wKey.isPressed) moveInput.y += 1;
        if (Keyboard.current.sKey.isPressed) moveInput.y -= 1;
        if (Keyboard.current.aKey.isPressed) moveInput.x -= 1;
        if (Keyboard.current.dKey.isPressed) moveInput.x += 1;
        
        sprintInput = Keyboard.current.leftShiftKey.isPressed;
        crouchInput = Keyboard.current.leftCtrlKey.isPressed;
        jumpInput = Keyboard.current.spaceKey.wasPressedThisFrame;

        // Geschwindigkeit basierend auf Input
        float currentSpeed = walkSpeed;
        if (crouchInput)
        {
            currentSpeed = crouchSpeed;
        }
        else if (sprintInput && moveInput.y > 0) // Nur beim Vorwärtslaufen sprinten
        {
            currentSpeed = sprintSpeed;
        }

        // Bewegung berechnen - relativ zur Blickrichtung
        Vector3 move = transform.right * moveInput.x + transform.forward * moveInput.y;
        characterController.Move(move * currentSpeed * Time.deltaTime);

        // Springen
        if (jumpInput && isGrounded)
        {
            velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
        }

        // Schwerkraft anwenden
        velocity.y += gravity * Time.deltaTime;
        characterController.Move(velocity * Time.deltaTime);
    }

    void HandleLook()
    {
        // Maus-Input lesen
        if (Cursor.lockState == CursorLockMode.Locked)
        {
            lookInput = Mouse.current.delta.ReadValue();
            
            // DEBUG: Zeige Werte an
            if (lookInput.magnitude > 0.1f)
            {
                Debug.Log("Maus Delta: " + lookInput + " | CameraPitch VORHER: " + cameraPitch);
            }
            
            // Horizontale Rotation (Y-Achse des Spielers)
            float yaw = lookInput.x * lookSensitivity;
            transform.Rotate(Vector3.up * yaw);

            // Vertikale Rotation (X-Achse der Kamera)
            // WICHTIG: Positiv = nach oben, Negativ = nach unten
            float pitchChange = lookInput.y * lookSensitivity;
            cameraPitch -= pitchChange;
            cameraPitch = Mathf.Clamp(cameraPitch, -maxLookAngle, maxLookAngle);
            
            if (lookInput.magnitude > 0.1f)
            {
                Debug.Log("Pitch Change: " + pitchChange + " | CameraPitch NACHHER: " + cameraPitch);
            }
            
            if (cameraTransform != null)
            {
                // Verwende nur X-Rotation für Pitch, Y und Z bleiben bei 0
                cameraTransform.localRotation = Quaternion.Euler(cameraPitch, 0f, 0f);
                
                if (lookInput.magnitude > 0.1f)
                {
                    Debug.Log("Kamera Rotation gesetzt auf: " + cameraTransform.localRotation.eulerAngles);
                }
            }
            else
            {
                Debug.LogError("cameraTransform ist NULL!");
            }

            // Arme folgen der Kamera-Rotation
            if (armsTransform != null)
            {
                armsTransform.localRotation = Quaternion.Euler(cameraPitch, 0f, 0f);
            }
        }
    }

    void HandleHeadBob()
    {
        if (!enableHeadBob || cameraTransform == null) return;

        // Prüfe ob sich der Spieler bewegt und am Boden ist
        Vector3 horizontalVelocity = new Vector3(characterController.velocity.x, 0, characterController.velocity.z);
        float speed = horizontalVelocity.magnitude;

        if (speed > 0.1f && isGrounded)
        {
            // Berechne Bob-Multiplikator basierend auf Bewegungsart
            float bobMultiplier = 1f;
            if (crouchInput)
            {
                bobMultiplier = 0.5f; // Weniger Bob beim Ducken
            }
            else if (sprintInput)
            {
                bobMultiplier = sprintBobMultiplier; // Mehr Bob beim Sprinten
            }

            // Timer für Sinuswelle
            bobTimer += Time.deltaTime * bobSpeed * bobMultiplier;

            // Berechne Bob-Offset (Sinuswelle für auf/ab, Cosinus für links/rechts)
            float bobOffsetY = Mathf.Sin(bobTimer) * bobAmount * bobMultiplier;
            float bobOffsetX = Mathf.Cos(bobTimer * 0.5f) * bobAmount * 0.5f * bobMultiplier;

            // Wende Bob auf Kamera an
            Vector3 targetPosition = originalCameraPosition + new Vector3(bobOffsetX, bobOffsetY, 0);
            cameraTransform.localPosition = Vector3.Lerp(cameraTransform.localPosition, targetPosition, Time.deltaTime * 10f);
        }
        else
        {
            // Kehre sanft zur Original-Position zurück wenn nicht bewegt
            bobTimer = 0f;
            cameraTransform.localPosition = Vector3.Lerp(cameraTransform.localPosition, originalCameraPosition, Time.deltaTime * 5f);
        }
    }
}
