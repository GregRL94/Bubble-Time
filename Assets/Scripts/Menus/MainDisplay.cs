using TMPro;
using UnityEngine;
using UnityEngine.UI;


public class MainDisplay : MonoBehaviour
{
    public TextMeshProUGUI LivesValue;
    public Button RestartButton;
    public Button QuitButton;
    public static MainDisplay Instance { get; private set; }
    private GameManager _gameManager;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else if (Instance != this)
        {
            Destroy(gameObject);
        }
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _gameManager = GameManager.Instance;
        LivesValue.text = _gameManager.MaxLives.ToString();
    }

    public void UpdateHealthDisplay(int currentHealth)
    {
        currentHealth = Mathf.Clamp(currentHealth, 0, _gameManager.MaxLives);
        LivesValue.text = currentHealth.ToString();
    }

    private void ShowGameOverMenu()
    {
        RestartButton.gameObject.SetActive(true);
        QuitButton.gameObject.SetActive(true);
    }

    private void HideGameOverMenu()
    {
        RestartButton.gameObject.SetActive(false);
        QuitButton.gameObject.SetActive(false);
    }

    public void OnRestartClicked()
    {
        HideGameOverMenu();
        _gameManager.RestartGame();
    }

    public void OnQuitClicked()
    {
        HideGameOverMenu();
        _gameManager.QuitGame();
    }

    private void OnEnable()
    {
        GameManager.OnGameOver += ShowGameOverMenu;
    }

    private void OnDisable()
    {
        GameManager.OnGameOver -= ShowGameOverMenu;
    }

    private void OnDestroy()
    {
        if (Instance == this)
        {
            Instance = null;
        }
        GameManager.OnGameOver -= ShowGameOverMenu;
    }
}
