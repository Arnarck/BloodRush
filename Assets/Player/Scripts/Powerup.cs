using System.Collections;
using UnityEngine;

public abstract class Powerup : MonoBehaviour
{
    Coroutine _countdownRoutine;

    bool _isActivated;

    public float Lifetime { get => lifeTime; }
    public bool IsActivated { get => _isActivated; protected set => _isActivated = value; }
    public Coroutine CountdownRoutine { get => _countdownRoutine; protected set => _countdownRoutine = value; }

    [SerializeField] float lifeTime;

    public abstract void Activate();

    protected abstract void Deactivate();

    protected abstract IEnumerator CountdownToDeactivate();
}
