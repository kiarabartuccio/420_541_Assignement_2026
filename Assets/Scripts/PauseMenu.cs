using UnityEngine;

public class PauseMenu : MonoBehaviour
{
    [SerializeField] private GameObject pausePanel;
    private bool paused;
    private void Start()
    {
        SetPaused(false);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            SetPaused(!paused);
        }
    }

    private void SetPaused(bool value)
    {
        paused = value;
        pausePanel.SetActive(paused);
        // 0 pauses physics timers
        Time.timeScale = paused ? 0f : 1f;
        // Release cursor so player can press menu buttons:
        Cursor.lockState = paused ? CursorLockMode.None : CursorLockMode.Locked;
        Cursor.visible = paused;
    }

    public void ResumeGame()
    {
        SetPaused(false);
    }

    public void RestartLevel()
    {
        SetPaused(false);
        GameManager.Instance.RestartLevel();
    }

public void QuitGame()
{
    // Restore normal time before closing the game.
    Time.timeScale = 1f;

#if UNITY_EDITOR
    // Stop Play mode when testing inside Unity.
    UnityEditor.EditorApplication.isPlaying = false;
#else
    // Close the application when playing the built game.
    Application.Quit();
#endif
}
}