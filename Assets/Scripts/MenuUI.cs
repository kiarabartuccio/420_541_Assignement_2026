using TMPro;
using UnityEngine;

public class MenuUI : MonoBehaviour
{
    // empty on MainMenu
    // Assign them on EndScreen
    [SerializeField] private TMP_Text finalScoreText;
    [SerializeField] private TMP_Text finalTimeText;

    private void Start()
    {
        // menu buttons are clickable:
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        if (finalScoreText != null)
        {
            finalScoreText.text = "Final Score: " + GameManager.Instance.Score;
        }

        if (finalTimeText != null)
        {
            int totalSeconds = Mathf.FloorToInt(
                GameManager.Instance.ElapsedTime
            );

            int minutes = totalSeconds / 60;
            int seconds = totalSeconds % 60;

            finalTimeText.text = $"Time: {minutes:00}:{seconds:00}";
        }
    }

    public void PlayGame()
    {
        GameManager.Instance.StartNewGame();
    }

    public void RestartGame()
    {
        // StartNewGame clears score and reloads Level1
        GameManager.Instance.StartNewGame();
    }

    public void QuitGame()
    {
        // Restore normal time before quitting.
        Time.timeScale = 1f;

    #if UNITY_EDITOR
        // Stop Play mode when testing inside Unity.
        UnityEditor.EditorApplication.isPlaying = false;
    #else
        // Close the application when running the built game.
        Application.Quit();
    #endif
    }
}
