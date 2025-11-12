using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody2D))]
public class playercontroler : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] private float moveSpeed = 6f;
    [SerializeField] private float jumpHeight = 2.5f;

    [Header("Ground Check")]
    [SerializeField] private Transform groundCheck;
    [SerializeField] private float groundRadius = 0.15f;
    [SerializeField] private LayerMask groundMask;

    [Header("Physics")]
    [SerializeField] private float gravity = -25f;

    [Header("Respawn")]
    [SerializeField] private float respawnThresholdY = -10f;
    [SerializeField] private Transform respawnPoint;

    [Header("Input (New System)")]
    [SerializeField] private InputActionReference moveActionReference;
    [SerializeField] private InputActionReference jumpActionReference;

    private Rigidbody2D body;
    private float verticalVelocity;
    private bool isGrounded;
    private Vector2 defaultSpawnPosition;
    private InputAction moveAction;
    private InputAction jumpAction;
    private bool jumpQueued;

    private void Awake()
    {
        body = GetComponent<Rigidbody2D>();
        body.gravityScale = 0f; // We handle gravity manually for consistent jump control.
        defaultSpawnPosition = respawnPoint != null ? (Vector2)respawnPoint.position : (Vector2)transform.position;
    }

    private void OnEnable()
    {
        BindInput(true);
    }

    private void OnDisable()
    {
        BindInput(false);
        jumpQueued = false;
    }

    private void Update()
    {
        HandleGroundCheck();
        HandleMovement();
        HandleRespawn();
    }

    private void HandleMovement()
    {
        float inputX = moveAction != null ? moveAction.ReadValue<Vector2>().x : 0f;
        Vector2 velocity = body.linearVelocity;
        velocity.x = inputX * moveSpeed;

        if (isGrounded)
        {
            verticalVelocity = verticalVelocity < 0f ? -2f : verticalVelocity;

            if (jumpQueued)
            {
                verticalVelocity = Mathf.Sqrt(jumpHeight * -2f * gravity);
                jumpQueued = false;
            }
        }
        else
        {
            jumpQueued = false;
        }

        verticalVelocity += gravity * Time.deltaTime;
        velocity.y = verticalVelocity;

        body.linearVelocity = velocity;
    }

    private void HandleGroundCheck()
    {
        if (groundCheck == null)
        {
            Debug.LogWarning($"{nameof(playercontroler)} is missing a ground check transform.", this);
            isGrounded = false;
            return;
        }

        isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundRadius, groundMask);
    }

    private void HandleRespawn()
    {
        if (transform.position.y <= respawnThresholdY)
        {
            Respawn();
        }
    }

    private void Respawn()
    {
        Vector2 targetPosition = respawnPoint != null ? (Vector2)respawnPoint.position : defaultSpawnPosition;
        body.position = targetPosition;
        body.linearVelocity = Vector2.zero;
        verticalVelocity = 0f;
    }

    private void OnDrawGizmosSelected()
    {
        if (groundCheck == null)
        {
            return;
        }

        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(groundCheck.position, groundRadius);
    }

    private void BindInput(bool enable)
    {
        if (enable)
        {
            moveAction = moveActionReference != null ? moveActionReference.action : null;
            jumpAction = jumpActionReference != null ? jumpActionReference.action : null;

            moveAction?.Enable();

            if (jumpAction != null)
            {
                jumpAction.started += OnJump;
                jumpAction.Enable();
            }
        }
        else
        {
            moveAction?.Disable();

            if (jumpAction != null)
            {
                jumpAction.started -= OnJump;
                jumpAction.Disable();
            }
        }
    }

    private void OnJump(InputAction.CallbackContext context)
    {
        if (context.phase == InputActionPhase.Started)
        {
            jumpQueued = true;
        }
    }
}
