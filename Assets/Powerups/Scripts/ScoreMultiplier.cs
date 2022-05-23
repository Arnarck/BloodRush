using System.Collections;
using UnityEngine;

public class ScoreMultiplier : Powerup
{
    float _startMultiplier;
    ScoreCounter _scoreCounter;

    [SerializeField] float multiplier = 2f;

    void Awake()
    {
        _scoreCounter = FindObjectOfType<ScoreCounter>();
    }

    public override void Activate()
    {
        if (IsActivated)
        {
            StopCoroutine(CountdownRoutine);
            CountdownRoutine = StartCoroutine(CountdownToDeactivate());
            return;
        }

        IsActivated = true;

        _startMultiplier = _scoreCounter.CurrentMultiplier;
        _scoreCounter.CurrentMultiplier = multiplier;
        CountdownRoutine = StartCoroutine(CountdownToDeactivate());
    }

    protected override IEnumerator CountdownToDeactivate()
    {
        yield return new WaitForSeconds(CurrentLifetime);
        Deactivate();
    }

    protected override void Deactivate()
    {
        IsActivated = false;
        _scoreCounter.CurrentMultiplier = _startMultiplier;
    }
}
