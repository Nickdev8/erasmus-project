using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody2D))]
public class playercontroler : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] private float walkSpeed = 6f;
    [SerializeField] private float runSpeed = 9.5f;
    [SerializeField] private float acceleration = 75f;
    [SerializeField] private float deceleration = 90f;
    [SerializeField, Range(0f, 1f)] private float airControlPercent = 0.5f;

    [Header("Jumping")]
    [SerializeField] private float jumpHeight = 2.8f;
    [SerializeField] private float coyoteTime = 0.15f;
    [SerializeField] private float jumpBufferTime = 0.15f;
    [SerializeField] private float fallGravityMultiplier = 2.5f;
    [SerializeField] private float lowJumpGravityMultiplier = 2f;

    [Header("Ground Check")]
    [SerializeField] private Transform groundCheck;
    [SerializeField] private float groundRadius = 0.15f;
    [SerializeField] private LayerMask groundMask;

    [Header("Physics")]
    [SerializeField] private float gravity = -25f;

    [Header("Respawn")]
    [SerializeField] private float respawnThresholdY = -10f;
    [SerializeField] private Transform respawnPoint;

    [Header("Audio")]
    [SerializeField] private string walkClipId = "foorsteps-normal";
    [SerializeField] private string runClipId = "foorsteps-running";
    [SerializeField] private float footstepVolume = 0.7f;
    [SerializeField] private float walkInterval = 0.25f;
    [SerializeField] private float runInterval = 0.18f;
    [SerializeField] private float minFootstepSpeed = 0.1f;

    [Header("Input (New System)")]
    [SerializeField] private InputActionReference moveActionReference;
    [SerializeField] private InputActionReference jumpActionReference;
    [SerializeField] private InputActionReference runActionReference;

    private Rigidbody2D body;
    private float verticalVelocity;
    private bool isGrounded;
    private Vector2 defaultSpawnPosition;
    private InputAction moveAction;
    private InputAction jumpAction;
    private InputAction runAction;
    private bool jumpQueued;
    private bool jumpHeld;
    private float coyoteCounter;
    private float jumpBufferCounter;
    private float footstepTimer;
    private AudioSource footstepSource;
    private string currentFootstepClipId;
    private void Awake()
    {

        body = GetComponent<Rigidbody2D>();
        body.gravityScale = 0f; // Custom gravity keeps jump timings consistent.
        defaultSpawnPosition = respawnPoint != null ? (Vector2)respawnPoint.position : (Vector2)transform.position;
        SetupFootstepSource();
    }

    private void OnEnable()
    {
        BindInput(true);
    }

    private void OnDisable()
    {
        BindInput(false);
        jumpQueued = false;
        jumpHeld = false;
        jumpBufferCounter = 0f;
        StopFootstepClip();
        currentFootstepClipId = null;
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
        bool wantsToRun = runAction != null && runAction.IsPressed();
        float targetSpeedMagnitude = wantsToRun ? runSpeed : walkSpeed;
        Vector2 velocity = body.linearVelocity;

        bool hasInput = !Mathf.Approximately(inputX, 0f);
        float targetSpeed = inputX * targetSpeedMagnitude;
        float accelRate = hasInput ? acceleration : deceleration;
        float controlPercent = isGrounded ? 1f : airControlPercent;

        velocity.x = Mathf.MoveTowards(velocity.x, targetSpeed, accelRate * controlPercent * Time.deltaTime);

        UpdateJumpState();

        if (jumpQueued && jumpBufferCounter > 0f && coyoteCounter > 0f)
        {
            PerformJump();
        }

        ApplyGravity();
        velocity.y = verticalVelocity;

        body.linearVelocity = velocity;
        HandleFootsteps(Mathf.Abs(velocity.x), wantsToRun);
    }

    private void UpdateJumpState()
    {
        if (isGrounded)
        {
            coyoteCounter = coyoteTime;
            if (verticalVelocity < 0f)
            {
                verticalVelocity = -2f; // small downward force to keep grounded
            }
        }
        else
        {
            coyoteCounter -= Time.deltaTime;
        }

        if (jumpBufferCounter > 0f)
        {
            jumpBufferCounter -= Time.deltaTime;
        }
        else
        {
            jumpQueued = false;
        }
    }

    private void PerformJump()
    {
        verticalVelocity = Mathf.Sqrt(jumpHeight * -2f * gravity);
        jumpQueued = false;
        jumpBufferCounter = 0f;
    }

    private void ApplyGravity()
    {
        float gravityScale = 1f;

        if (verticalVelocity < 0f)
        {
            gravityScale = fallGravityMultiplier;
        }
        else if (!jumpHeld)
        {
            gravityScale = lowJumpGravityMultiplier;
        }

        verticalVelocity += gravity * gravityScale * Time.deltaTime;
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
        coyoteCounter = 0f;
        jumpBufferCounter = 0f;
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
            runAction = runActionReference != null ? runActionReference.action : null;

            moveAction?.Enable();

            if (jumpAction != null)
            {
                jumpAction.started += OnJumpStarted;
                jumpAction.canceled += OnJumpCanceled;
                jumpAction.Enable();
            }

            runAction?.Enable();
        }
        else
        {
            moveAction?.Disable();
            
            if (jumpAction != null)
            {
                jumpAction.started -= OnJumpStarted;
                jumpAction.canceled -= OnJumpCanceled;
                jumpAction.Disable();
            }

            if (runAction != null)
            {
                runAction.Disable();
                runAction = null;
            }
        }
    }

    private void OnJumpStarted(InputAction.CallbackContext context)
    {
        jumpQueued = true;
        jumpHeld = true;
        jumpBufferCounter = jumpBufferTime;
    }

    private void OnJumpCanceled(InputAction.CallbackContext context)
    {
        jumpHeld = false;
    }

    private void HandleFootsteps(float horizontalSpeed, bool isRunning)
    {
        string clipId = isRunning ? runClipId : walkClipId;
        float interval = isRunning ? runInterval : walkInterval;
        bool canPlay = isGrounded && horizontalSpeed >= minFootstepSpeed && !string.IsNullOrWhiteSpace(clipId);

        if (!canPlay)
        {
            StopFootstepClip();
            footstepTimer = 0f;
            currentFootstepClipId = null;
            return;
        }

        footstepTimer -= Time.deltaTime;
        float clampedInterval = Mathf.Max(0.05f, interval);

        if (currentFootstepClipId != clipId)
        {
            PlayFootstepClip(clipId, restart: true);
            currentFootstepClipId = clipId;
            footstepTimer = clampedInterval;
            return;
        }

        if (footstepTimer <= 0f)
        {
            PlayFootstepClip(clipId, restart: true);
            footstepTimer = clampedInterval;
        }
    }

    private void SetupFootstepSource()
    {
        footstepSource = gameObject.AddComponent<AudioSource>();
        footstepSource.playOnAwake = false;
        footstepSource.loop = false;
        footstepSource.spatialBlend = 0f;
    }

    private void PlayFootstepClip(string clipId, bool restart)
    {
        if (footstepSource == null)
        {
            return;
        }

        AudioClip clip = AudioManager.Instance != null ? AudioManager.Instance.GetClip(clipId) : null;

        if (clip == null)
        {
            AudioManager.Instance?.PlaySFX(clipId, transform.position, footstepVolume);
            return;
        }

        if (footstepSource.clip != clip)
        {
            footstepSource.clip = clip;
        }

        footstepSource.volume = footstepVolume;
        footstepSource.pitch = 1f;
        if (restart)
        {
            footstepSource.Stop();
            footstepSource.time = 0f;
        }

        footstepSource.Play();
    }

    private void StopFootstepClip()
    {
        if (footstepSource != null && footstepSource.isPlaying)
        {
            footstepSource.Stop();
        }
    }


}
