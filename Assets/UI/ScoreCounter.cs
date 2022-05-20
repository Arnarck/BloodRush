using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ScoreCounter : MonoBehaviour
{
    int _score, _currentReduceRate;
    float _currentDelay, _currentMultiplier;

    [SerializeField] TextMeshProUGUI scoreDisplay;

    [Header("Reduce Update Settings")]
    [Tooltip("The score needed to reduce the time to update.")]
    [SerializeField] int scoreToReduceUpdateTime = 100;

    [Tooltip("The time reduced to update the score.")]
    [SerializeField] float timeReduced = .05f;

    [Header("Update Settings")]
    [SerializeField] float startMultiplier = 1f;

    [Tooltip("The initial time until the score increase again.")]
    [SerializeField] float timeToUpdate = .25f;

    [Tooltip("The minimum time to update accepted.")]
    [SerializeField] float minTimeToUpdate = .05f;

    public static ScoreCounter Instance { get; private set; }

    public int Score { get => _score; private set => _score = value; }
    public int CurrentReduceRate { get => _currentReduceRate; private set => _currentReduceRate = value; }
    public float CurrentTimeToUpdate { get => _currentDelay; private set => _currentDelay = value; }
    public float CurrentMultiplier { get => _currentMultiplier; private set => _currentMultiplier = value; }

    void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        CurrentReduceRate = scoreToReduceUpdateTime;
        CurrentTimeToUpdate = timeToUpdate;
        CurrentMultiplier = startMultiplier;

        StartCoroutine(IncrementScore());
    }

    IEnumerator IncrementScore()
    {
        while (!GameOver.Instance.IsGameOver)
        {
            Score++;
            if (Score >= CurrentReduceRate)
            {
                ReduceUpdateTime();
            }

            scoreDisplay.text = Score.ToString();
            yield return new WaitForSeconds(CurrentTimeToUpdate / CurrentMultiplier);
        }
    }

    void ReduceUpdateTime()
    {
        CurrentReduceRate += scoreToReduceUpdateTime;
        CurrentTimeToUpdate -= timeReduced;

        CurrentTimeToUpdate = Mathf.Clamp(CurrentTimeToUpdate, minTimeToUpdate, timeToUpdate);
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
