using UnityEngine;
using System.Collections;

public class BerserkerMode : MonoBehaviour
{
    BerserkerBar _bar;
    Vignette _vignette;
    PlayerGravity _gravity;
    PlayerMovement _movement;
    ScoreCounter _scoreCounter;
    PropGenerator _propGenerator;
    SpeedProgression _speedProgression;

    bool _isTransformed;

    [SerializeField] float dodgeSpeedIncreased = 2.5f;
    [SerializeField] float speedIncreased = 1.2f;
    [SerializeField] float spawnTimeReduced = .05f;
    [SerializeField] float scoreTimeReduced = .05f;
    [SerializeField] float timeToConsumeBar = .5f;
    [SerializeField] Color skyboxColor;
    [SerializeField] Color fogColor;

    public bool IsTransformed { get => _isTransformed; private set => _isTransformed = value; }

    void Awake()
    {
        _bar = FindObjectOfType<BerserkerBar>();
        _vignette = FindObjectOfType<Vignette>();
        _gravity = FindObjectOfType<PlayerGravity>();
        _movement = FindObjectOfType<PlayerMovement>();
        _scoreCounter = FindObjectOfType<ScoreCounter>();
        _propGenerator = FindObjectOfType<PropGenerator>();
        _speedProgression = FindObjectOfType<SpeedProgression>();
    }

    public void Activate()
    {
        if (_bar.CurrentValue < _bar.MaxValue) return;
        if (_isTransformed) return;

        _isTransformed = true;

        _gravity.ForwardSpeed += speedIncreased;
        _propGenerator.TimeToSpawn -= spawnTimeReduced;
        _scoreCounter.CurrentTimeToUpdate -= scoreTimeReduced;
        _movement.DodgeSpeed += dodgeSpeedIncreased;
        _vignette.Activate();

        RenderSettings.skybox.SetColor("_SkyTint", skyboxColor);
        RenderSettings.fogColor = fogColor;

        StartCoroutine(ConsumeBarOverTime());
    }

    public void ReduceBarValue(int amount)
    {
        _bar.CurrentValue -= amount;

        if (_bar.CurrentValue < 1)
        {
            Deactivate();
        }
    }

    IEnumerator ConsumeBarOverTime()
    {
        while (IsTransformed)
        {
            yield return new WaitForSeconds(timeToConsumeBar);
            ReduceBarValue(1);
        }
    }

    void Deactivate()
    {
        IsTransformed = false;

        StopAllCoroutines();
        _gravity.ForwardSpeed -= speedIncreased;
        _propGenerator.TimeToSpawn += spawnTimeReduced;
        _scoreCounter.CurrentTimeToUpdate += scoreTimeReduced;
        _movement.DodgeSpeed -= dodgeSpeedIncreased;
        _vignette.Deactivate();

        _speedProgression.SetSkyboxColor();
    }
}
