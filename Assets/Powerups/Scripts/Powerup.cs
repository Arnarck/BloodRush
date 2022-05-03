using System.Collections;
using UnityEngine;

public abstract class Powerup : MonoBehaviour
{
    Coroutine _countdownRoutine;

    bool _isActivated;
    float _currentLifetime;

    public float InitialLifetime { get => initialLifetime; }
    public bool IsActivated { get => _isActivated; protected set => _isActivated = value; }
    public float CurrentLifetime { get => _currentLifetime; private set => _currentLifetime = value; }
    public Coroutine CountdownRoutine { get => _countdownRoutine; protected set => _countdownRoutine = value; }

    [SerializeField] float initialLifetime;
    [SerializeField] SaveData.Powerup powerupName;

    void Start()
    {
        // Formula is 3.5x + 5. "initialLifetime" is the constant value (5).
        float x = SaveData.GetPowerupLevel(powerupName);
        CurrentLifetime = 3.5f * x + initialLifetime;
        Debug.Log(powerupName + ": " + CurrentLifetime);
    }

    public abstract void Activate();

    protected abstract void Deactivate();

    protected abstract IEnumerator CountdownToDeactivate();
}
