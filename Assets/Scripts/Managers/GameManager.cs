using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class GameManager : MonoBehaviour
{
    [Header("Player Lives & Respawn Settings")]
    [field:SerializeField, Tooltip("The maximum number of lives of the player")] public int MaxLives { get; private set;} = 3;
    [field:SerializeField, Tooltip("The time in seconds before the player is respawned after death")] public float RespawnTime { get; private set;} = 2f;
    [field:SerializeField, Tooltip("The time in seconds before the GameOver screen is shown after the player lost all lives")] public float GameOverTime { get; private set;} = 3f;

    public static Action OnPlayerRespawnCompleted;
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

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _player = GameObject.FindGameObjectWithTag("Player");
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
}
