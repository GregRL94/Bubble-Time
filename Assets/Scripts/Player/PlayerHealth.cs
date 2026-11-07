using System.Collections;
using UnityEngine;

public class PlayerHealth : MonoBehaviour, IHit
{
    [Header("Player Lives Settings")]
    [SerializeField, Tooltip("The maximum number of lives of the player")] private int _maxLives = 3;
    [SerializeField, Tooltip("The time in seconds before the player is respawned after death")] private float _respawnTime = 2f;
    [SerializeField, Tooltip("The time in seconds before the GameOver screen is shown after the player lost all lives")] private float _gameOverTime = 3f;

    public bool IsDead { get; set; } = false;
    private int _currentLives;
    private bool _isDead;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _currentLives = _maxLives;
    }

    private void LooseLife()
    {
        if (_isDead) return;

        _currentLives--;
        if (_currentLives > 0)
        {
            Debug.Log("Lost a life! Remaining lives: " + _currentLives);
            StartCoroutine(Die(_respawnTime));
            return;
        }
        Debug.Log("No remaining lives! Game Over!");
        StartCoroutine(Die(_gameOverTime));
    }

    public void GainLife()
    {
        if (_currentLives < _maxLives)
        {
            _currentLives++;
            Debug.Log("Gained a life! Current lives: " + _currentLives);
        }
        else
        {
            Debug.Log("Already at maximum lives!");
        }
    }

    IEnumerator Die(float waitForSeconds)
    {
        _isDead = true;
        // TODO:Disable Controls
        // TODO:Play Death Animation
        // TODO:Play Death Sound
        yield return new WaitForSeconds(waitForSeconds);
        if (_currentLives > 0)
        {
            Debug.Log("Respawn player");
            // TODO: GameManager Respawn Player
        }
        else
        {
            Debug.Log("Show Game Over Screen");
            // TODO: GameManager Show GameOver Screen
        }
    }

    public void OnHit()
    {
        LooseLife();
    }

    public void OnHit(float _) { }
}
