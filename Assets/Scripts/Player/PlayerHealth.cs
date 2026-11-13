using System.Collections;
using UnityEngine;

public class PlayerHealth : MonoBehaviour, IHit
{
    public bool IsDead { get; set; } = false;
    private SpriteRenderer _spriteRenderer;
    private int _maxLives => GameManager.Instance.MaxLives;
    private float _respawnTime => GameManager.Instance.RespawnTime;
    private float _gameOverTime => GameManager.Instance.GameOverTime;
    private int _currentLives;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (!TryGetComponent(out _spriteRenderer))
        {
            Debug.LogError("SpriteRenderer component not found on Player!");
        }
        _currentLives = _maxLives;
        Subscribe(true);
    }

    private void LooseLife()
    {
        if (IsDead) return;

        _currentLives--;
        // TODO: Play a sound effect for losing a life
        // TODO: Play a visual effect for losing a life (e.g., flash the screen, show a UI element, etc.)
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

    private void OnRespawn()
    {
        _spriteRenderer.enabled = true;
        PlayerInputHandler.Instance.EnablePLayerInputs(true);
        IsDead = false;
    }

    IEnumerator Die(float waitForSeconds)
    {
        IsDead = true;
        _spriteRenderer.enabled = false;
        PlayerInputHandler.Instance.EnablePLayerInputs(false);
        // TODO:Play Death Animation. Use body supparts from the object pooler to create a ragdoll effect. Use the RagdollSettings class to configure the ragdoll effect.
        // TODO:Play Death Sound
        yield return new WaitForSeconds(waitForSeconds);
        if (_currentLives >= 0)
        {
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
