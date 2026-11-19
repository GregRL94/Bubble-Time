using System.Collections;
using UnityEngine;

public class PlayerHealth : MonoBehaviour, IHit
{
    public bool IsDead { get; set; } = false;
    private SpriteRenderer _spriteRenderer;
    private Animator _animator;
    private int _maxLives => GameManager.Instance.MaxLives;
    private float _respawnTime => GameManager.Instance.RespawnTime;
    private float _gameOverTime => GameManager.Instance.GameOverTime;
    private int _currentLives;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _spriteRenderer = GetComponent<SpriteRenderer>();
        _animator = GetComponent<Animator>();
        _currentLives = _maxLives;
        Subscribe(true);
    }

    private void LooseLife()
    {
        if (IsDead) return;

        _currentLives--;
        MainDisplay.Instance.UpdateHealthDisplay(_currentLives);
        // TODO: Play a sound effect for losing a life
        // TODO: Play a visual effect for losing a life
        if (_currentLives >= 0)
        {
            Debug.Log("Lost a life! Remaining lives: " + _currentLives);
            StartCoroutine(Die(_respawnTime));
            return;
        }
        Debug.Log("No remaining lives!");
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

    private void OnRespawn()
    {
        _spriteRenderer.enabled = true;
        _animator.SetTrigger("Respawn");
        PlayerInputHandler.Instance.EnablePLayerInputs(true);
        IsDead = false;
    }

    IEnumerator Die(float waitForSeconds)
    {
        IsDead = true;
        PlayerInputHandler.Instance.EnablePLayerInputs(false);
        _animator.SetTrigger("Die");
        // TODO:Play Death Sound
        yield return new WaitForSeconds(waitForSeconds);
        if (_currentLives >= 0)
        {
            _spriteRenderer.enabled = false;
            GameManager.Instance.RespawnPlayer();
        }
        else
        {
            GameManager.Instance.GameOver();
        }
    }

    private void Subscribe(bool enable)
    {
        if (enable)
        {
            GameManager.OnPlayerRespawnCompleted += OnRespawn;
        }
        else
        {
            GameManager.OnPlayerRespawnCompleted -= OnRespawn;
        }
    }

    private void OnDisable()
    {
        Subscribe(false);
    }

    private void OnDestroy()
    {
        Subscribe(false);
    }

    public void OnHit(float _) { LooseLife(); }
}
