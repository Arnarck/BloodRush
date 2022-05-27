using UnityEngine;
using TMPro;

public class GameOver : MonoBehaviour
{
    bool _isGameOver;

    public static GameOver Instance { get; private set; }

    [SerializeField] GameObject gameOverScreen;
    [SerializeField] TextMeshProUGUI scoreDisplay;
    [SerializeField] TextMeshProUGUI bloodDisplay;
    [SerializeField] TextMeshProUGUI highscoreDisplay;

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

        scoreDisplay.text = ScoreCounter.Instance.Score.ToString();
        bloodDisplay.text = FindObjectOfType<PlayerCollision>().BloodCollected.ToString();
        highscoreDisplay.text = SaveData.GetInventoryData(SaveData.PlayerInventory.Highscore).ToString();
        ScoreCounter.Instance.SaveHighScore();
    }
}
