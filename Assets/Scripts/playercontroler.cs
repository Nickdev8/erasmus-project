using UnityEngine;

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

    private Rigidbody2D body;
    private float verticalVelocity;
    private bool isGrounded;
    private Vector2 defaultSpawnPosition;

    private void Awake()
    {
        body = GetComponent<Rigidbody2D>();
        body.gravityScale = 0f; // We handle gravity manually for consistent jump control.
        defaultSpawnPosition = respawnPoint != null ? (Vector2)respawnPoint.position : (Vector2)transform.position;
    }

    private void Update()
    {
        HandleGroundCheck();
        HandleMovement();
        HandleRespawn();
    }

    private void HandleMovement()
    {
        float inputX = Input.GetAxisRaw("Horizontal");
        Vector2 velocity = body.linearVelocity;
        velocity.x = inputX * moveSpeed;

        if (isGrounded)
        {
            verticalVelocity = verticalVelocity < 0f ? -2f : verticalVelocity;

            if (Input.GetButtonDown("Jump"))
            {
                verticalVelocity = Mathf.Sqrt(jumpHeight * -2f * gravity);
            }
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
}
