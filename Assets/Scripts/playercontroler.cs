using UnityEngine;

[RequireComponent(typeof(CharacterController))]
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

    private CharacterController controller;
    private float verticalVelocity;
    private bool isGrounded;

    private void Awake()
    {
        controller = GetComponent<CharacterController>();
    }

    private void Update()
    {
        HandleGroundCheck();
        HandleMovement();
    }

    private void HandleMovement()
    {
        float inputX = Input.GetAxisRaw("Horizontal");
        Vector3 move = Vector3.right * inputX;

        if (isGrounded)
        {
            verticalVelocity = verticalVelocity < 0f ? -2f : verticalVelocity;

            if (Input.GetButtonDown("Jump"))
            {
                verticalVelocity = Mathf.Sqrt(jumpHeight * -2f * gravity);
            }
        }

        verticalVelocity += gravity * Time.deltaTime;
        move.y = verticalVelocity;

        controller.Move(moveSpeed * Time.deltaTime * move);
    }

    private void HandleGroundCheck()
    {
        if (groundCheck == null)
        {
            Debug.LogWarning($"{nameof(playercontroler)} is missing a ground check transform.", this);
            isGrounded = false;
            return;
        }

        isGrounded = Physics.CheckSphere(groundCheck.position, groundRadius, groundMask, QueryTriggerInteraction.Ignore);
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
