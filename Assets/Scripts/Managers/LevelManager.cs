using System;
using UnityEngine;

public class LevelManager : MonoBehaviour
{
    public static Action OnRespawnCompleted;
    public static Action OnLevelCompleted;
    public static Action OnGameOverCompleted;

    [field: SerializeField, Tooltip("A list of all the points where the player can respawn in the level. Order matters.")] public Transform[] Checkpoints { get; private set; }
    public static LevelManager Instance { get; private set; }
    public int CurrentCheckpointIndex { get; private set; } = 0;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else if (Instance != this)
        {
            Destroy(this.gameObject);
        }
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        GameManager.Instance.RegisterLevel(this);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void NextCheckpoint()
    {
        if (CurrentCheckpointIndex < Checkpoints.Length - 1)
        {
            CurrentCheckpointIndex++;
        }
    }


    private void OnDestroy()
    {
        GameManager.Instance.UnregisterLevel(this);
    }
}
