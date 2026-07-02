using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PlayerController : MonoBehaviour
{
    private InteractableObject currentInteractable;

    [Header("Movement")]
    public float speed = 5f;
    public float gravity = -9.81f;

    [Header("Jump")]
    public float jumpHeight = 2f;

    private bool isGrounded;

    [Header("Mouse Look")]
    public float mouseSensitivity = 100f;
    public Transform playerCamera;

    [Header("Mobile")]
    public Joystick joystick;

    [Header("Interaction")]
    public float interactDistance = 3f;
    public LayerMask interactLayer;

    [Header("UI")]
    public Image crosshair;
    public Color normalColor = Color.white;
    public Color interactColor = Color.green;

    public TextMeshProUGUI interactText;

    [Header("Ground Check")]
    public Transform groundCheck;
    public float groundDistance = 0.3f;
    public LayerMask groundMask;

    private CharacterController controller;
    private Animator animator;

    private float xRotation = 0f;
    private Vector3 velocity;

    // External Forces
    [Header("External Forces")]
    public float forceDamping = 5f;

    private Vector3 externalForce;

    // Head Bob
    [Header("Head Bob")]
    public float bobSpeed = 8f;
    public float bobAmount = 0.05f;

    private float defaultCameraY;
    private float bobTimer;

    // Sprint FOV
    [Header("Sprint FOV")]
    public Camera cam;
    public float normalFOV = 60f;
    public float sprintFOV = 72f;
    public float fovSmoothness = 8f;

    // Landing Bump
    [Header("Landing")]
    public float landingDipAmount = 0.15f;
    public float landingRecoverSpeed = 6f;

    private bool wasGrounded;
    private Vector3 cameraOriginalPos;
    private float landingOffset;

    private Vector3 currentMoveVelocity;

    void Start()
    {
        controller = GetComponent<CharacterController>();
  
        animator = GetComponentInChildren<Animator>();
    
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        defaultCameraY = playerCamera.localPosition.y;
        cameraOriginalPos = playerCamera.localPosition;

    }

    void Update()
    {
        Look();
        Move();
        Animate();
        Interact();
        HeadBob();
        SprintFOV();
        LandingBump();

        if (Input.GetKeyDown(KeyCode.F))
        {
            AddForce(-transform.forward, 8f);
        }

    }

    // =========================
    // MOVEMENT
    // =========================


    void Move()
    {
        float x = Input.GetAxis("Horizontal");
        float z = Input.GetAxis("Vertical");

        // Mobile joystick support
        if (joystick != null)
        {
            x += joystick.Horizontal;
            z += joystick.Vertical;
        }

        Vector3 inputDirection = transform.right * x + transform.forward * z;

        inputDirection = Vector3.ClampMagnitude(inputDirection, 1f);

        // Sprint
        bool isSprinting = Input.GetKey(KeyCode.LeftShift);

        float targetSpeed = isSprinting ? speed * 1.8f : speed;

        Vector3 targetMove = inputDirection * targetSpeed;

        // Smooth acceleration
        currentMoveVelocity = Vector3.Lerp(
            currentMoveVelocity,
            targetMove,
            10f * Time.deltaTime
        );

        // Apply damping to force
        externalForce = Vector3.Lerp(
            externalForce,
            Vector3.zero,
            forceDamping * Time.deltaTime
        );

        Vector3 finalMove =
            currentMoveVelocity + externalForce;

        controller.Move(
            finalMove * Time.deltaTime
        );

        // Ground Check

        isGrounded =
            controller.isGrounded ||
            Physics.CheckSphere(
                groundCheck.position,
                groundDistance,
                groundMask
            );  

        if (isGrounded && velocity.y < 0)
        {
            velocity.y = -1f;
        }

        // Jump
        if (Input.GetKeyDown(KeyCode.Space) && isGrounded)
        {
            velocity.y = Mathf.Sqrt(
                jumpHeight * -2f * gravity
            );

            animator.SetTrigger("Jump");
        }

        // Gravity
        velocity.y += gravity * Time.deltaTime;

        controller.Move(velocity * Time.deltaTime);
    }

    void OnDrawGizmosSelected()
    {
        if (groundCheck == null) return;

        Gizmos.color = Color.green;

        Gizmos.DrawSphere(
            groundCheck.position,
            groundDistance
        );
    }

    // =========================
    // CAMERA LOOK
    // =========================
    void Look()
    {
        float mouseX =
            Input.GetAxis("Mouse X") *
            mouseSensitivity *
            Time.deltaTime;

        float mouseY =
            Input.GetAxis("Mouse Y") *
            mouseSensitivity *
            Time.deltaTime;

        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -80f, 80f);

        // Camera up/down
        playerCamera.localRotation =
            Quaternion.Euler(xRotation, 0f, 0f);

        // Player left/right
        transform.Rotate(Vector3.up * mouseX);
    }

    // =========================
    // ANIMATION
    // =========================

    void Animate()
    {
        Vector3 horizontalVelocity =
            new Vector3(
                currentMoveVelocity.x,
                0,
                currentMoveVelocity.z
            );

        float animationSpeed =
            horizontalVelocity.magnitude / (speed * 1.8f);

        animator.SetFloat(
            "Speed",
            animationSpeed,
            0.1f,
            Time.deltaTime
        );
    }

    void HeadBob()
    {
        if (!isGrounded)
            return;

        Vector3 horizontalVelocity =
            new Vector3(
                currentMoveVelocity.x,
                0,
                currentMoveVelocity.z
            );

        if (horizontalVelocity.magnitude > 0.1f)
        {
            bobTimer += Time.deltaTime * bobSpeed;

            Vector3 newPosition =
                playerCamera.localPosition;

            newPosition.y =
                defaultCameraY +
                Mathf.Sin(bobTimer) * bobAmount -
                landingOffset;

            playerCamera.localPosition =
                newPosition;
        }
        else
        {
            bobTimer = 0;

            Vector3 resetPosition =
                playerCamera.localPosition;

            resetPosition.y = Mathf.Lerp(
                resetPosition.y,
                defaultCameraY,
                Time.deltaTime * 8f
            );

            playerCamera.localPosition =
                resetPosition;
        }
    }

    void SprintFOV()
    {
        bool isMoving =
            currentMoveVelocity.magnitude > 0.1f;

        bool isSprinting =
            Input.GetKey(KeyCode.LeftShift);

        float targetFOV =
            (isMoving && isSprinting)
            ? sprintFOV
            : normalFOV;

        cam.fieldOfView = Mathf.Lerp(
            cam.fieldOfView,
            targetFOV,
            fovSmoothness * Time.deltaTime
        );
    }

    void LandingBump()
    {
        // Detect landing
        if (!wasGrounded && isGrounded)
        {
            landingOffset = landingDipAmount;
        }

        wasGrounded = isGrounded;

        // Smoothly recover
        landingOffset = Mathf.Lerp(
            landingOffset,
            0,
            landingRecoverSpeed * Time.deltaTime
        );

        Vector3 targetPosition =
            playerCamera.localPosition;

        targetPosition.y -= landingOffset;

        playerCamera.localPosition =
            targetPosition;
    }

    public void AddForce(Vector3 direction, float strength)
    {
        externalForce += direction.normalized * strength;
    }

    // =========================
    // INTERACTION
    // =========================
    void Interact()
    {
        Ray ray = new Ray(
            playerCamera.position,
            playerCamera.forward
        );

        RaycastHit hit;

        Debug.DrawRay(
            playerCamera.position,
            playerCamera.forward * interactDistance,
            Color.red
        );

        if (Physics.Raycast(
            ray,
            out hit,
            interactDistance,
            interactLayer))
        {
            crosshair.color = interactColor;

            interactText.text =
                "[E] Interact";

            InteractableObject interactable =
                hit.collider.GetComponent<InteractableObject>();

            if (interactable != null)
            {
                if (currentInteractable != interactable)
                {
                    if (currentInteractable != null)
                    {
                        currentInteractable.UnHighlight();
                    }

                    currentInteractable = interactable;
                    currentInteractable.Highlight();
                }
            }

            if (Input.GetKeyDown(KeyCode.E))
            {
                Debug.Log("Interacted with: " + hit.collider.name);

                hit.collider.SendMessage(
                    "Interact",
                    SendMessageOptions.DontRequireReceiver
                );
            }
        }
        else
        {
            crosshair.color = normalColor;

            interactText.text = "";

            if (currentInteractable != null)
            {
                currentInteractable.UnHighlight();
                currentInteractable = null;
            }
        }    
    }
}
