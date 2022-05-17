using UnityEngine;

public class GameOver : MonoBehaviour
{
    bool _isGameOver;

    public static GameOver Instance { get; private set; }

    [SerializeField] GameObject gameOverScreen;

    public bool IsGameOver { get => _isGameOver; private set => _isGameOver = value; }

    void Awake()
    {
        Instance = this;
    }

    public void Activate()
    {
        PauseGame.Instance.SetPause(true);
        gameOverScreen.SetActive(true);
        IsGameOver = true;

        ScoreCounter.Instance.SaveHighScore();
    }
}
