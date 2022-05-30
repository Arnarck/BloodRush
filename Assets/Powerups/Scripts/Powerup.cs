using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public abstract class Powerup : MonoBehaviour
{
    Coroutine _countdownRoutine;

    bool _isActivated;
    float _currentLifetime;

    public float InitialLifetime { get => initialLifetime; }
    public bool IsActivated { get => _isActivated; protected set => _isActivated = value; }
    public float CurrentLifetime { get => _currentLifetime; private set => _currentLifetime = value; }
    public Coroutine CountdownRoutine { get => _countdownRoutine; protected set => _countdownRoutine = value; }
    public Slider HealthBar { get => healthBar; protected set => healthBar = value; }

    [SerializeField] float initialLifetime;
    [SerializeField] Slider healthBar;
    [SerializeField] SaveData.Powerup powerupName;
    [SerializeField] protected SoundType sfx;
    [SerializeField] protected SoundManager.SoundCaster caster;

    void Start()
    {
        float powerupLevel = SaveData.GetPowerupLevel(powerupName);
        CurrentLifetime = initialLifetime * powerupLevel;

        HealthBar.gameObject.SetActive(false);
        HealthBar.maxValue = CurrentLifetime;
        HealthBar.value = CurrentLifetime;
    }

    public abstract void Activate();

    protected abstract void Deactivate();

    protected abstract IEnumerator CountdownToDeactivate();
}
