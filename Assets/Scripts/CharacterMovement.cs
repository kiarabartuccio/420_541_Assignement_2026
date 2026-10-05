using UnityEngine;
//uses Lab 4 and 5
// script needs Rigidbody on same GameObject:
[RequireComponent(typeof(Rigidbody))]
public class CharacterMovement : MonoBehaviour
{
    [Header("Movement Settings")]
    [SerializeField] private float walkSpeed = 4f;
    [SerializeField] private float runMultiplier = 2f;

    // Here, it makes movement 50 percent faster.
    [SerializeField] private float speedBoostMultiplier = 1.5f;

    [Header("Camera")]
    [SerializeField] private Transform cameraTransform;

    [Header("Ground Check")]
    [SerializeField] private Transform groundCheck;
    [SerializeField] private LayerMask groundMask;
    [SerializeField] private float groundRadius = 0.18f;

    [Header("Jump Settings")]
    [SerializeField] private float minimumJumpSpeed = 5f;
    [SerializeField] private float maximumJumpSpeed = 9f;
    [SerializeField] private float maximumChargeTime = 3f;
    [SerializeField] private float doubleJumpSpeed = 8f;

    // Other scripts can read these properties:
    public bool IsGrounded => isGrounded;

    // Increases each time a double jump is done
    // CharacterAnimator is used to detect new double jump
    public int DoubleJumpCount { get; private set; }

    public float SpeedTimeLeft => Mathf.Max(0f, speedBoostEndTime - Time.time);

    public float ScoreTimeLeft => Mathf.Max(0f, scoreBoostEndTime - Time.time);

    public float JumpTimeLeft => Mathf.Max(0f, jumpBoostEndTime - Time.time);

    // Purple pickup changes yellow pickup rewards from 50 to 100:
    public int ScoreMultiplier => ScoreTimeLeft > 0f ? 2 : 1;

    private Rigidbody rb;
    private Vector3 moveDirection;
    private bool running;
    private bool isGrounded;

    private bool chargingJump;
    private float chargeTime;

    // Update prepares the jump and FixedUpdate performs it:
    private bool jumpRequested;
    private bool requestedDoubleJump;
    private float requestedJumpSpeed;

    private bool doubleJumpUsed;
    private float ignoreGroundUntil;

    private float speedBoostEndTime;
    private float scoreBoostEndTime;
    private float jumpBoostEndTime;

    private void Awake()
    {
        // Get Rigidbody once instead of looking at every frame:
        rb = GetComponent<Rigidbody>();
    }

    private void Update()
    {
        // Ignore gameplay input when paused:
        if (Time.timeScale == 0f)
        {
            moveDirection = Vector3.zero;
            // Cancel unfinished charge when pausing:
            chargingJump = false;
            chargeTime = 0f;
            jumpRequested = false;

            return;
        }
        CheckGround();
        ReadMovement();
        ReadJump();
    }

    private void CheckGround()
    {
        // right after a jump, the feet can still be close to the floor.
        // this short delay prevents false treatment of landing.
        isGrounded = Time.time >= ignoreGroundUntil && Physics.CheckSphere(
                groundCheck.position,
                groundRadius,
                groundMask,
                QueryTriggerInteraction.Ignore
            );

        // Landing lets one new double jump for next airtime:
        if (isGrounded && !jumpRequested)
        {
            doubleJumpUsed = false;
        }
    }

    private void ReadMovement()
    {
        float horizontal = Input.GetAxisRaw("Horizontal");
        float vertical = Input.GetAxisRaw("Vertical");

        // Camera view make W move toward the view:
        Vector3 forward = cameraTransform.forward;
        Vector3 right = cameraTransform.right;

        // Camera tilt doesnt push player upward/downward:
        forward.y = 0f;
        right.y = 0f;

        forward.Normalize();
        right.Normalize();

        moveDirection = forward * vertical + right * horizontal;

        // doesnt allow diagonal movement from being faster:
        moveDirection = Vector3.ClampMagnitude(moveDirection, 1f);

        running = Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift);
    }

    private void ReadJump()
    {
        if (Input.GetKeyDown(KeyCode.Space) && !jumpRequested)
        {
            if (isGrounded)
            {
                // Start a new charged jump.
                chargingJump = true;
                chargeTime = 0f;
            }
            else if (JumpTimeLeft > 0f && !doubleJumpUsed)
            {
                // Double jump happens right with fixed strength.
                doubleJumpUsed = true;
                RequestJump(doubleJumpSpeed, true);
            }
        }

        if (!chargingJump)
        {
            return;
        }

        // Walking off a platform cancels the ground jump:
        if (!isGrounded)
        {
            chargingJump = false;
            chargeTime = 0f;
            return;
        }

        chargeTime += Time.deltaTime;

        // Launch when released, or automatically after three seconds:
        if (Input.GetKeyUp(KeyCode.Space) || chargeTime >= maximumChargeTime)
        {
            float chargePercentage = Mathf.Clamp01(chargeTime / maximumChargeTime);
            // quick release gives minimumJumpSpeed
            // full charge gives maximumJumpSpeed
            float jumpSpeed = Mathf.Lerp(
                minimumJumpSpeed,
                maximumJumpSpeed,
                chargePercentage
            );

            RequestJump(jumpSpeed, false);
            chargingJump = false;
            chargeTime = 0f;
        }
    }

    private void RequestJump(float speed, bool isDoubleJump)
    {
        // Store the request for next physics update:
        requestedJumpSpeed = speed;
        requestedDoubleJump = isDoubleJump;
        jumpRequested = true;
    }

    private void FixedUpdate()
    {
        MovePlayer();

        if (jumpRequested)
        {
            Jump();
            jumpRequested = false;
        }
    }

    private void MovePlayer()
    {
        float currentSpeed = walkSpeed;

        if (running)
        {
            currentSpeed *= runMultiplier;
        }

        if (SpeedTimeLeft > 0f)
        {
            currentSpeed *= speedBoostMultiplier;
        }

        Vector3 targetVelocity = moveDirection * currentSpeed;

        // Y velocity belongs to gravity and jumping:
        rb.linearVelocity = new Vector3(
            targetVelocity.x,
            rb.linearVelocity.y,
            targetVelocity.z
        );

        if (moveDirection.sqrMagnitude > 0.01f)
        {
            // Rotate through the Rigidbody not Transform
            Quaternion targetRotation = Quaternion.LookRotation(moveDirection);
            rb.MoveRotation(targetRotation);
        }
    }

    private void Jump()
    {
        // Clear previous falling/rising speed for consistent launch:
        rb.linearVelocity = new Vector3(
            rb.linearVelocity.x,
            0f,
            rb.linearVelocity.z
        );

        // Lab 5: immediate velocity change.
        rb.AddForce(
            Vector3.up * requestedJumpSpeed, ForceMode.VelocityChange
        );

        isGrounded = false;
        ignoreGroundUntil = Time.time + 0.15f;

        if (requestedDoubleJump)
        {
            DoubleJumpCount++;
        }

        requestedDoubleJump = false;
    }

    public void ActivateSpeedBoost()
    {
        // Collecting another boost refreshes time:
        speedBoostEndTime = Time.time + 5f;
    }

    public void ActivateScoreBoost()
    {
        scoreBoostEndTime = Time.time + 10f;
    }

    public void ActivateJumpBoost()
    {
        jumpBoostEndTime = Time.time + 30f;
    }

    private void OnDrawGizmosSelected()
    {
        // Show the ground-check sphere when Player is selected:
        if (groundCheck == null)
        {
            return;
        }

        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(groundCheck.position, groundRadius);
    }
}
