using System.Collections;
using UnityEngine;

public class HigherJump : Powerup
{
    CameraFollower _camera;
    PlayerGravity _gravity;

    Coroutine _toggleHigherJumpCamera;

    [SerializeField] float force = 20f;
    [SerializeField][Range(0f, 1f)] float cameraOffset = .6f;

    void Awake()
    {
        _gravity = GetComponent<PlayerGravity>();
        _camera = FindObjectOfType<CameraFollower>();
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

        if (_toggleHigherJumpCamera != null)
        {
            StopCoroutine(_toggleHigherJumpCamera);
        }

        IsActivated = true;

        _gravity.SetJumpForce(force);
        CountdownRoutine = StartCoroutine(CountdownToDeactivate());
        _toggleHigherJumpCamera = StartCoroutine(ToggleHigherJumpCamera(true));
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
        _gravity.ResetJumpForce();
        _toggleHigherJumpCamera = StartCoroutine(ToggleHigherJumpCamera(false));
    }

    IEnumerator ToggleHigherJumpCamera(bool willActivate)
    {
        while (!_gravity.IsGrounded)
        {
            yield return new WaitForEndOfFrame();
        }

        if (willActivate) _camera.SetJumpOffset(cameraOffset);
        else _camera.ResetJumpOffset();
    }
}
