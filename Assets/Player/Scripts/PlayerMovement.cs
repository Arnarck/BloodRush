using System.Collections;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    PlayerGravity _gravity;
    PlayerController _controller;
    Coroutine _moveTo, _slideRoutine, _autoSlideRoutine;

    bool _isDodging, _isSliding, _isAutoSlideEnabled, _isDodgingToRight;

    public bool IsDodging { get => _isDodging; }
    public bool IsSliding { get => _isSliding; }
    public bool IsDodgingToRight { get => _isDodgingToRight; }
    public bool IsAutoSlideEnabled { get => _isAutoSlideEnabled; }
    public float DodgeSpeed { get => dodgeSpeed; set => dodgeSpeed = value; }

    [Header("Sound Effects")]
    [SerializeField] SoundManager.SoundCaster soundCaster;
    [SerializeField] SoundType slideSFX;

    [Header("Visual Effects")]
    [SerializeField] ParticleType slideVFX;

    [Header("Movement Settings")]
    [SerializeField] float dodgeSpeed = 5f;
    [SerializeField] float slideTime = 2f;

    void Awake()
    {
        _gravity = GetComponent<PlayerGravity>();
        _controller = GetComponent<PlayerController>();
    }

    public void DodgeTo(float xPosition)
    {
        if (_isDodging) StopCoroutine(_moveTo);

        _moveTo = StartCoroutine(MoveTo(xPosition));
    }

    public void MoveToPreviousLane()
    {
        if (!_isDodging) return;

        _controller.SwitchLane(_controller.PreviousLane);
    }

    public void ApplySlide()
    {
        if (_isSliding) return;

        _slideRoutine = StartCoroutine(Slide());
    }

    public void CancelSlide()
    {
        if (!_isSliding) return;

        _controller.PlayerAnimator.SetBool("isRolling", false);
        StopCoroutine(_slideRoutine);
        ParticleManager.Stop(slideVFX);
        SoundManager.instance.StopSound(slideSFX, soundCaster);
        _isSliding = false;
    }

    public void ApplyAutoSlide()
    {
        if (_isAutoSlideEnabled) return;
        
        _autoSlideRoutine = StartCoroutine(AutoSlide());
    }

    public void CancelAutoSlide()
    {
        if (!_isAutoSlideEnabled) return;

        StopCoroutine(_autoSlideRoutine);
        _isAutoSlideEnabled = false;
    }

    IEnumerator Slide()
    {
        ParticleManager.Play(slideVFX, _gravity.ForwardSpeed);
        _controller.PlayerAnimator.SetBool("isRolling", true);
        SoundManager.instance.PlaySound(slideSFX, soundCaster, true);
        _isSliding = true;

        yield return new WaitForSeconds(slideTime);

        ParticleManager.Stop(slideVFX);
        _controller.PlayerAnimator.SetBool("isRolling", false);
        SoundManager.instance.StopSound(slideSFX, soundCaster);
        _isSliding = false;
    }

    IEnumerator AutoSlide()
    {
        _isAutoSlideEnabled = true;
        while (!_gravity.IsGrounded)
        {
            yield return new WaitForEndOfFrame();
        }
        _isAutoSlideEnabled = false;

        _slideRoutine = StartCoroutine(Slide());
    }

    IEnumerator MoveTo(float endPosition)
    {
        float startPosition = transform.position.x;
        float movementPercentage = 0f;

        _isDodging = true;
        _isDodgingToRight = endPosition > startPosition ? true : false;
        while (movementPercentage < 1f)
        {
            float xPosition;
            float movementThisFrame = Time.deltaTime * dodgeSpeed;

            movementPercentage += movementThisFrame;
            xPosition = Mathf.Lerp(startPosition, endPosition, movementPercentage);
            transform.position = new Vector3(xPosition, transform.position.y, transform.position.z);

            yield return new WaitForEndOfFrame();
        }
        _isDodging = false;
    }
}
