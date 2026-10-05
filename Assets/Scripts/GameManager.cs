using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    // shared manager can access from other scripts:
    public static GameManager Instance { get; private set; }

    public int Score { get; private set; }
    public float ElapsedTime { get; private set; }

    // Prevent alot of collisions from requesting many scene loads:
    private bool loadingScene;
    private bool gameplayScene;

    // Score checkpoint for restarting a failed level:
    private int scoreAtLevelStart;

    private void Awake()
    {
        // Remove any extra manager created in later scene:
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;

        // manager = separate root GameObject:
        DontDestroyOnLoad(gameObject);

        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void Start()
    {
        // Allows testing by opening gameplay scene right away:
        ConfigureScene(SceneManager.GetActiveScene().name);
    }

    private void Update()
    {
        if (gameplayScene)
        {
            // Does not advance while Time.timeScale = zero:
            ElapsedTime += Time.deltaTime;
        }
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        ConfigureScene(scene.name);
    }

    private void ConfigureScene(string sceneName)
    {
        loadingScene = false;
        Time.timeScale = 1f;
        gameplayScene =
            sceneName == "Level1" || sceneName == "Level2" || sceneName == "Level3";
        if (gameplayScene)
        {
            scoreAtLevelStart = Score;
        }
        else
        {
            // Menu buttons need shows unlocked cursor.
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
    }

    public void AddScore(int amount)
    {
        Score += amount;
        Debug.Log("Score: " + Score);
    }

    public void StartNewGame()
    {
        Score = 0;
        ElapsedTime = 0f;
        scoreAtLevelStart = 0;
        LoadScene("Level1");
    }

    public void RestartLevel()
    {
        if (loadingScene)
        {
            return;
        }

        // Remove points earned during failed try:
        Score = scoreAtLevelStart;

        LoadScene(SceneManager.GetActiveScene().name);
    }

    public void CompleteLevel(string nextSceneName)
    {
        if (loadingScene)
        {
            return;
        }

        // Score = unchanged when moving to the next level:
        LoadScene(nextSceneName);
    }

    private void LoadScene(string sceneName)
    {
        loadingScene = true;
        // Resets pause before loading:
        Time.timeScale = 1f;
        SceneManager.LoadScene(sceneName);
    }

    public void QuitGame()
    {
        Application.Quit();
    }

    private void OnDestroy()
    {
        if (Instance == this)
        {
            SceneManager.sceneLoaded -= OnSceneLoaded;
            Instance = null;
        }
    }
}
