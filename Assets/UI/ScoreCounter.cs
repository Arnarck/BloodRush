using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ScoreCounter : MonoBehaviour
{
    int _score;
    float _currentDelay, _currentMultiplier;

    [SerializeField] TextMeshProUGUI scoreDisplay;
    [SerializeField] float startMultiplier = 1f;
    [Tooltip("How Much the cooldown will decrease for each speed gain")][SerializeField] float delayDecrease = .02f;
    [Tooltip("The initial cooldown time to the score update again")][SerializeField] float startDelay = .25f;
    [SerializeField] float minDelay = .01f;

    public static ScoreCounter Instance { get; private set; }

    public int Score { get => _score; private set => _score = value; }
    public float CurrentDelay { get => _currentDelay; private set => _currentDelay = value; }
    public float CurrentMultiplier { get => _currentMultiplier; private set => _currentMultiplier = value; }

    void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        CurrentDelay = startDelay;
        CurrentMultiplier = startMultiplier;
        StartCoroutine(IncrementScore());
    }

    IEnumerator IncrementScore()
    {
        while (!GameOver.Instance.IsGameOver)
        {
            Score++;
            scoreDisplay.text = Score.ToString();
            yield return new WaitForSeconds(CurrentDelay / CurrentMultiplier);
        }
    }

    void DecreaseDelay()
    {
        CurrentDelay -= delayDecrease;
    }

    void SetScoreMultiplier(float multiplier)
    {
        CurrentMultiplier = multiplier;
    }

    void ResetScoreMultiplier()
    {
        CurrentMultiplier = startMultiplier;
    }

    public void SaveHighScore()
    {
        int highScore = SaveData.GetInventoryData(SaveData.PlayerInventory.Highscore);
        if (Score > highScore)
        {
            SaveData.SetHighScore(Score);
        }
    }
}
