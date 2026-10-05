using TMPro;
using UnityEngine;

public class HUD : MonoBehaviour
{
    [Header("Text Objects")]
    [SerializeField] private TMP_Text scoreText;
    [SerializeField] private TMP_Text healthText;
    [SerializeField] private TMP_Text timerText;
    [SerializeField] private TMP_Text powerupText;

    [Header("Player")]
    [SerializeField] private PlayerHealth playerHealth;
    [SerializeField] private CharacterMovement playerMovement;

    private void Update()
    {
        scoreText.text = "Score: " + GameManager.Instance.Score;
        healthText.text = "Health: " + playerHealth.CurrentHealth + " / 3";

        // Convert total seconds into minutes with seconds:
        int totalSeconds = Mathf.FloorToInt(GameManager.Instance.ElapsedTime);
        int minutes = totalSeconds / 60;
        int seconds = totalSeconds % 60;
        timerText.text = $"Time: {minutes:00}:{seconds:00}";
        string powerupInformation = "";

        if (playerMovement.SpeedTimeLeft > 0f)
        {
            powerupInformation += $"Speed boost: {playerMovement.SpeedTimeLeft:0.0}s\n";
        }

        if (playerMovement.ScoreTimeLeft > 0f)
        {
            powerupInformation += $"Double score: {playerMovement.ScoreTimeLeft:0.0}s\n";
        }

        if (playerMovement.JumpTimeLeft > 0f)
        {
            powerupInformation += $"Double jump: {playerMovement.JumpTimeLeft:0.0}s";
        }

        powerupText.text = powerupInformation;
    }
}
