using UnityEngine;

public class Trap : MonoBehaviour
{
    private void OnCollisionEnter(Collision collision)
    {
        // Find PlayerHealth on colliding object or parent:
        PlayerHealth health = collision.collider.GetComponentInParent<PlayerHealth>();

        if (health != null)
        {
            health.TakeDamage(1);
        }
    }
}
