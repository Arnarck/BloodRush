using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ScoreCounter : MonoBehaviour
{
    SpeedProgression _speedProgression;

    int _score, _currentReduceRate;
    float _currentDelay, _currentMultiplier;

    [SerializeField] TextMeshProUGUI scoreDisplay;

    [Header("Reduce Update Settings")]
    [SerializeField] float scoreToIncreaseSpeed = 200;
    [SerializeField] float secondScoreToIncreaseSpeed = 800;

    [Tooltip("The time reduced to update the score.")]
    [SerializeField] float timeReduced = .05f;

    [Header("Update Settings")]
    [SerializeField] float startMultiplier = 1f;

    [Tooltip("The initial time until the score increase again.")]
    [SerializeField] float timeToUpdate = .25f;

    public static ScoreCounter Instance { get; private set; }

    public int Score { get => _score; private set => _score = value; }
    public int CurrentReduceRate { get => _currentReduceRate; private set => _currentReduceRate = value; }
    public float CurrentTimeToUpdate { get => _currentDelay; set => _currentDelay = value; }
    public float CurrentMultiplier { get => _currentMultiplier; set => _currentMultiplier = value; }

    void Awake()
    {
        Instance = this;
        _speedProgression = FindObjectOfType<SpeedProgression>();
    }

    void Start()
    {
        CurrentTimeToUpdate = timeToUpdate;
        CurrentMultiplier = startMultiplier;

        StartCoroutine(IncrementScore());
    }

    IEnumerator IncrementScore()
    {
        while (!GameOver.Instance.IsGameOver)
        {
            Score++;

            if (Score == scoreToIncreaseSpeed)
            {
                ReduceUpdateTime();
                _speedProgression.IncreaseSpeed();
            }

            if (Score == secondScoreToIncreaseSpeed)
            {
                ReduceUpdateTime();
                _speedProgression.IncreaseSpeed();
            }

            scoreDisplay.text = Score.ToString();
            yield return new WaitForSeconds(CurrentTimeToUpdate / CurrentMultiplier);
        }
    }

    void ReduceUpdateTime()
    {
        CurrentTimeToUpdate -= timeReduced;
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
