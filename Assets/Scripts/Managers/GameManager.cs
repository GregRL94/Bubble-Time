using UnityEngine;
using System;

public class GameManager : MonoBehaviour
{
    [Header("Player Lives & Respawn Settings")]
    [field:SerializeField, Tooltip("The maximum number of lives of the player")] public int MaxLives { get; private set;} = 3;
    [field:SerializeField, Tooltip("The time in seconds before the player is respawned after death")] public float RespawnTime { get; private set;} = 2f;
    [field:SerializeField, Tooltip("The time in seconds before the GameOver screen is shown after the player lost all lives")] public float GameOverTime { get; private set;} = 3f;

    public static Action OnPlayerRespawnCompleted;
    public static Action OnGameOver;
    public static Action OnGameRestart;
    public static GameManager Instance { get; private set; }

    private GameObject _player;
    private LevelManager _currentLevel;


    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else if (Instance != this)
        {
            Destroy(gameObject);
        }
    }

    public void RespawnPlayer()
    {
        Debug.Log("Respawn player");
        _player.transform.position = _currentLevel.Checkpoints[_currentLevel.CurrentCheckpointIndex].position;
        OnPlayerRespawnCompleted?.Invoke();
    }

    public void GameOver()
    {
        Debug.Log("Game Over");
        Time.timeScale = 0f;
        ShowCursor();
        OnGameOver?.Invoke();
    }

    public void RestartGame()
    {
        Debug.Log("Restart Game");
        Time.timeScale = 1f;
        ShowCursor(false);
        UnityEngine.SceneManagement.SceneManager.LoadScene(UnityEngine.SceneManagement.SceneManager.GetActiveScene().buildIndex);
    }

    public void QuitGame()
    {
        Debug.Log("Quit Game");
        Application.Quit();
    }

    public void RegisterPlayer(GameObject player)
    {
        _player = player;
        Debug.Log("Player registered: " + player.name);
    }

    public void UnregisterPlayer()
    {
        _player = null;
    }

    public void RegisterLevel(LevelManager level)
    {
        _currentLevel = level;
        Debug.Log("Level registered: " + level.name);
    }

    public void UnregisterLevel(LevelManager level)
    {
        _currentLevel = null;
    }

    private void ShowCursor(bool show=true)
    {
        Cursor.lockState = show ? CursorLockMode.None : CursorLockMode.Locked;
        Cursor.visible = show;
    }
}
