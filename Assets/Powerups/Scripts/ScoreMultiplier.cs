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
        SoundManager.instance.PlaySound(sfx, caster, false);
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
        float lifeTime = CurrentLifetime;
        HealthBar.gameObject.SetActive(true);
        while (IsActivated)
        {
            lifeTime -= Time.deltaTime;
            lifeTime = Mathf.Clamp(lifeTime, 0f, CurrentLifetime);
            HealthBar.value = lifeTime;
            if (lifeTime < Mathf.Epsilon)
            {
                Deactivate();
            }

            yield return new WaitForEndOfFrame();
        }
    }

    protected override void Deactivate()
    {
        IsActivated = false;
        HealthBar.gameObject.SetActive(false);
        _scoreCounter.CurrentMultiplier = _startMultiplier;
    }
}
