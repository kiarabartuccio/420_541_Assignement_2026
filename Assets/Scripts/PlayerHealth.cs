using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    [SerializeField] private int maximumHealth = 3;
    [SerializeField] private float damageCooldown = 1f;
    [SerializeField] private float fallHeight = -10f;

    public int CurrentHealth { get; private set; }
    private float nextDamageTime;
    private bool restarting;

    private void Awake()
    {
        // A new player is created when level loads:
        CurrentHealth = maximumHealth;
    }

    private void Update()
    {
        // Below this height = player falls off level:
        if (!restarting && transform.position.y < fallHeight)
        {
            RestartLevel();
        }
    }

    public void TakeDamage(int amount)
    {
        // helps no collision events from removing all health at one time:
        if (restarting || Time.time < nextDamageTime)
        {
            return;
        }

        nextDamageTime = Time.time + damageCooldown;
        CurrentHealth = Mathf.Max(0, CurrentHealth - amount);
        Debug.Log("Health: " + CurrentHealth);

        if (CurrentHealth == 0)
        {
            RestartLevel();
        }
    }

    private void RestartLevel()
    {
        restarting = true;
        GameManager.Instance.RestartLevel();
    }
}