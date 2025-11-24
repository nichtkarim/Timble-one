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
    public float gravity = -19.62f; // Erhöhte Gravitation für besseres Gefühl
    public float slopeForce = 8f; // Kraft um auf Slopes zu bleiben
    public float slopeForceRayLength = 2f;
    
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
        
        // CharacterController Setup prüfen
        if (characterController != null)
        {
            // Stelle sicher dass die Werte korrekt sind
            characterController.height = 2f;
            characterController.radius = 0.5f;
            characterController.center = new Vector3(0, 1, 0);
            
            Debug.Log("✅ CharacterController: Height=" + characterController.height + ", Radius=" + characterController.radius + ", Center=" + characterController.center);
        }
        else
        {
            Debug.LogError("❌ KEIN CHARACTER CONTROLLER GEFUNDEN!");
        }
        
        // Prüfe Gravity-Wert
        if (Mathf.Abs(gravity) < 1f)
        {
            Debug.LogError("❌ GRAVITY IST 0 ODER ZU KLEIN! Aktuell: " + gravity + " - Setze auf -19.62");
            gravity = -19.62f;
        }
        
        // Cursor verstecken und im Spiel sperren
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        
        Debug.Log("=== FIRSTPERSON CONTROLLER START ===");
        Debug.Log("Gravity: " + gravity);
        Debug.Log("CameraTransform zugewiesen? " + (cameraTransform != null));
        
        // Speichere Original-Position der Kamera und setze Rotation zurück
        if (cameraTransform != null)
        {
            originalCameraPosition = cameraTransform.localPosition;
            // Setze Kamera-Rotation auf 0 beim Start
            cameraTransform.localRotation = Quaternion.identity;
            cameraPitch = 0f;
            Debug.Log("✅ Kamera gefunden: " + cameraTransform.name);
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
        // Verbesserter Ground Check - nur am Boden wenn wirklich am Boden
        isGrounded = false;
        
        // Raycast nach unten vom Center des Controllers
        RaycastHit groundHit;
        float rayDistance = (characterController.height / 2f) + 0.1f;
        Vector3 rayOrigin = transform.position + characterController.center;
        
        if (Physics.Raycast(rayOrigin, Vector3.down, out groundHit, rayDistance))
        {
            isGrounded = true;
        }
        
        // Zusätzlicher Check mit CharacterController.isGrounded
        if (characterController.isGrounded)
        {
            isGrounded = true;
        }
        
        // DEBUG
        if (Time.frameCount % 60 == 0) // Jede Sekunde
        {
            Debug.Log("🌍 isGrounded: " + isGrounded + " | velocity.y: " + velocity.y + " | Position.y: " + transform.position.y);
        }
        
        // Nur velocity zurücksetzen wenn WIRKLICH am Boden
        if (isGrounded && velocity.y < 0)
        {
            velocity.y = -2f; // Kleine negative Kraft um am Boden zu bleiben
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
        else if (sprintInput && moveInput.y > 0)
        {
            currentSpeed = sprintSpeed;
        }

        // Bewegung berechnen - relativ zur Blickrichtung
        Vector3 move = transform.right * moveInput.x + transform.forward * moveInput.y;
        
        // Normalisiere Bewegung um diagonale Geschwindigkeit zu verhindern
        if (move.magnitude > 1f)
        {
            move.Normalize();
        }
        
        characterController.Move(move * currentSpeed * Time.deltaTime);

        // Slope Force - drückt Spieler nach unten auf Slopes
        if (isGrounded && move.magnitude > 0.1f)
        {
            RaycastHit slopeHit;
            Vector3 slopeRayOrigin = transform.position + characterController.center;
            if (Physics.Raycast(slopeRayOrigin, Vector3.down, out slopeHit, slopeForceRayLength))
            {
                if (slopeHit.normal != Vector3.up)
                {
                    // Wende zusätzliche Kraft nach unten an auf Slopes
                    characterController.Move(Vector3.down * slopeForce * Time.deltaTime);
                }
            }
        }

        // Springen
        if (jumpInput && isGrounded)
        {
            velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
            Debug.Log("🦘 SPRUNG! velocity.y = " + velocity.y);
        }

        // Schwerkraft IMMER anwenden - auch wenn am Boden!
        if (!isGrounded || velocity.y > 0) // Nur wenn in der Luft oder nach oben springt
        {
            velocity.y += gravity * Time.deltaTime;
        }
        
        // DEBUG - Zeige velocity vor Move
        if (Time.frameCount % 60 == 0)
        {
            Debug.Log("📉 Applying vertical velocity: " + velocity.y + " | Move: " + (velocity * Time.deltaTime));
        }
        
        // Vertikale Bewegung anwenden
        Vector3 verticalMove = new Vector3(0, velocity.y, 0) * Time.deltaTime;
        characterController.Move(verticalMove);
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
