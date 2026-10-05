using UnityEngine;

[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(CharacterMovement))]
[RequireComponent(typeof(Rigidbody))]
public class CharacterAnimator : MonoBehaviour
{
    [Header("Double Jump Effect")]
    [SerializeField] private AudioSource effectAudio;
    [SerializeField] private AudioClip doubleJumpSound;

    private Animator animator;
    private CharacterMovement movement;
    private Rigidbody rb;

    private int previousDoubleJumpCount;

    private void Awake()
    {
        // components all on Player:
        animator = GetComponent<Animator>();
        movement = GetComponent<CharacterMovement>();
        rb = GetComponent<Rigidbody>();
    }

    private void Update()
    {
        Vector3 velocity = rb.linearVelocity;
        // Ignore vertical velocity so jumping does not look like running:
        float horizontalSpeed = new Vector2(
            velocity.x,
            velocity.z
        ).magnitude;

        // Damping makes animation change smoothly:
        animator.SetFloat(
            "CharacterSpeed",
            horizontalSpeed,
            0.1f,
            Time.deltaTime
        );

        animator.SetBool("IsGrounded", movement.IsGrounded);

        // A larger number means movement script performed a double jump:
        if (movement.DoubleJumpCount != previousDoubleJumpCount)
        {
            previousDoubleJumpCount = movement.DoubleJumpCount;
            animator.SetTrigger("DoubleJump");

            if (effectAudio != null && doubleJumpSound != null)
            {
                effectAudio.PlayOneShot(doubleJumpSound);
            }
        }
    }
}