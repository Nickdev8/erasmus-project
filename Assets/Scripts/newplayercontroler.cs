using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody2D))]
public class newplayercontroler : MonoBehaviour
{
    [Header("Ground Check")]
    [SerializeField] private Transform groundCheck;
    [SerializeField] private float groundRadius = 0.15f;
    [SerializeField] private LayerMask groundMask;

    [Header("Scooter Movement")]
    [SerializeField] private float maxSpeed = 12f;
    [SerializeField] private float acceleration = 25f;
    [SerializeField] private float brakeDeceleration = 35f;
    [SerializeField] private float leanTorque = 8f;
    [SerializeField] private float autoBalanceStrength = 6f;
    [SerializeField] private float hopForce = 6.5f;
    [SerializeField] private float gravityScale = 3f;

    [Header("Respawn")]
    [SerializeField] private float respawnThresholdY = -10f;
    [SerializeField] private Transform respawnPoint;

    [Header("Input (New System)")]
    [SerializeField] private InputActionReference throttleActionReference;
    [SerializeField] private InputActionReference brakeActionReference;
    [SerializeField] private InputActionReference leanActionReference;
    [SerializeField] private InputActionReference hopActionReference;

    private Rigidbody2D body;
    private bool isGrounded;
    private Vector2 defaultSpawnPosition;
    private InputAction throttleAction;
    private InputAction brakeAction;
    private InputAction leanAction;
    private InputAction hopAction;
    private bool hopQueued;

    private void Awake()
    {
        body = GetComponent<Rigidbody2D>();
        body.gravityScale = gravityScale;
        body.interpolation = RigidbodyInterpolation2D.Interpolate;
        body.freezeRotation = false;
        defaultSpawnPosition = respawnPoint != null ? (Vector2)respawnPoint.position : (Vector2)transform.position;
    }

    private void OnEnable()
    {
        BindInput(true);
    }

    private void OnDisable()
    {
        BindInput(false);
        hopQueued = false;
    }

    private void FixedUpdate()
    {
        HandleGroundCheck();
        HandleScooterMovement();
        HandleHop();
        HandleRespawn();
    }

    private void HandleScooterMovement()
    {
        float throttleInput = ReadFloat(throttleAction);
        float brakeInput = Mathf.Clamp01(ReadFloat(brakeAction));
        float leanInput = ReadFloat(leanAction);

        Vector2 velocity = body.linearVelocity;
        float targetSpeed = throttleInput * maxSpeed;
        float accelStep = acceleration * Time.fixedDeltaTime;
        velocity.x = Mathf.MoveTowards(velocity.x, targetSpeed, accelStep);

        if (brakeInput > 0f)
        {
            float brakeStep = brakeInput * brakeDeceleration * Time.fixedDeltaTime;
            velocity.x = Mathf.MoveTowards(velocity.x, 0f, brakeStep);
        }

        velocity.x = Mathf.Clamp(velocity.x, -maxSpeed, maxSpeed);
        body.linearVelocity = velocity;

        ApplyLean(leanInput);
    }

    private void HandleGroundCheck()
    {
        if (groundCheck == null)
        {
            Debug.LogWarning($"{nameof(newplayercontroler)} is missing a ground check transform.", this);
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
        body.angularVelocity = 0f;
        body.SetRotation(0f);
    }

    private void HandleHop()
    {
        if (!hopQueued || !isGrounded)
        {
            return;
        }

        Vector2 velocity = body.linearVelocity;
        velocity.y = hopForce;
        body.linearVelocity = velocity;
        hopQueued = false;
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
            throttleAction = EnableAction(throttleActionReference);
            brakeAction = EnableAction(brakeActionReference);
            leanAction = EnableAction(leanActionReference);
            hopAction = EnableAction(hopActionReference);

            if (hopAction != null)
            {
                hopAction.started += OnHopStarted;
            }
        }
        else
        {
            if (hopAction != null)
            {
                hopAction.started -= OnHopStarted;
            }

            DisableAction(throttleAction);
            DisableAction(brakeAction);
            DisableAction(leanAction);
            DisableAction(hopAction);
        }
    }

    private void OnHopStarted(InputAction.CallbackContext context)
    {
        hopQueued = true;
    }

    private InputAction EnableAction(InputActionReference reference)
    {
        if (reference == null)
        {
            return null;
        }

        InputAction action = reference.action;
        action?.Enable();
        return action;
    }

    private void DisableAction(InputAction action)
    {
        action?.Disable();
    }

    private float ReadFloat(InputAction action)
    {
        return action != null ? action.ReadValue<float>() : 0f;
    }

    private void ApplyLean(float leanInput)
    {
        if (Mathf.Abs(leanInput) > 0.01f)
        {
            float torque = -leanInput * leanTorque * Time.fixedDeltaTime;
            body.AddTorque(torque, ForceMode2D.Force);
        }

        float uprightCorrection = Mathf.DeltaAngle(body.rotation, 0f) * autoBalanceStrength * Time.fixedDeltaTime;
        body.AddTorque(uprightCorrection, ForceMode2D.Force);
    }
}
