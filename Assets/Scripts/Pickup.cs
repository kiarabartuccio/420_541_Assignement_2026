using UnityEngine;

[RequireComponent(typeof(SphereCollider))]
public class Pickup : MonoBehaviour
{
    public enum PickupType
    {
        Speed,
        ScoreBoost,
        JumpBoost,
        Score
    }

    [Header("Pickup Settings")]
    [SerializeField] private PickupType type;
    [SerializeField] private int points = 50;
    [SerializeField] private float respawnDelay = 30f;

    [Header("Visual Movement")]
    [SerializeField] private Transform visuals;
    [SerializeField] private float rotationSpeed = 90f;
    [SerializeField] private float hoverHeight = 0.2f;
    [SerializeField] private float hoverSpeed = 2f;

    [Header("Sound")]
    [SerializeField] private AudioClip pickupSound;

    private Collider triggerCollider;
    private Vector3 startingVisualPosition;
    private bool available = true;

    private void Awake()
    {
        triggerCollider = GetComponent<Collider>();
        // Remembers child's original position:
        startingVisualPosition = visuals.localPosition;
    }

    private void Update()
    {
        if (!available)
        {
            return;
        }

        // Rotate only shown child keeping the trigger = stationary
        visuals.Rotate(
            Vector3.up,
            rotationSpeed * Time.deltaTime,
            Space.Self
        );

        // Sine gives a smooth up-and-down movement:
        float heightOffset = Mathf.Sin(Time.time * hoverSpeed) * hoverHeight;

        visuals.localPosition = startingVisualPosition + Vector3.up * heightOffset;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!available)
        {
            return;
        }

        CharacterMovement player = other.GetComponentInParent<CharacterMovement>();

        if (player == null)
        {
            return;
        }

        // Disable right away so pickup cannot be collected again
        available = false;
        triggerCollider.enabled = false;

        PlayPickupSound();

        switch (type)
        {
            case PickupType.Speed:
                player.ActivateSpeedBoost();
                break;

            case PickupType.ScoreBoost:
                player.ActivateScoreBoost();
                break;

            case PickupType.JumpBoost:
                player.ActivateJumpBoost();
                break;

            case PickupType.Score:
                GameManager.Instance.AddScore(
                    points * player.ScoreMultiplier
                );
                break;
        }

        if (type == PickupType.Score)
        {
            // Yellow pickups are removed from this try
            Destroy(gameObject);
        }
        else
        {
            // Hide only child. Keep the script's parent alive
            visuals.gameObject.SetActive(false);

            // Lab 4: delayed method-call:
            Invoke(nameof(Respawn), respawnDelay);
        }
    }

    private void Respawn()
    {
        visuals.gameObject.SetActive(true);
        triggerCollider.enabled = true;
        available = true;
    }

    private void PlayPickupSound()
    {
        if (pickupSound == null)
        {
            return;
        }

        // Use a separate temp object so the sound continues even when a yellow pickup is destroyed
        GameObject soundObject = new GameObject("PickupSound");
        AudioSource source = soundObject.AddComponent<AudioSource>();
        source.clip = pickupSound;
        source.spatialBlend = 0f;
        source.Play();
        // Delete the temp sound object after playback
        Destroy(soundObject, pickupSound.length + 0.1f);
    }
}