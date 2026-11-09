using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class GameManager : MonoBehaviour
{
    public static Action OnPlayerRespawn;
    public static GameManager Instance { get; private set; }


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
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void RespawnPlayer()
    {

    }

    private void GameOver()
    {

    }

    public void RegisterLevel(LevelManager level)
    {

    }

    public void UnregisterLevel(LevelManager level)
    {

    }

}
