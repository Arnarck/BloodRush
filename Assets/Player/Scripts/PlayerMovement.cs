using System.Collections;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    PlayerGravity _gravity;
    Coroutine _moveTo, _slideRoutine, _autoSlideRoutine;

    bool _isDodging, _isSliding, _isAutoSlideEnabled, _isDodgingToRight;

    public bool IsDodging { get => _isDodging; }
    public bool IsSliding { get => _isSliding; }
    public bool IsDodgingToRight { get => _isDodgingToRight; }
    public bool IsAutoSlideEnabled { get => _isAutoSlideEnabled; }

    [Header("Effects")]
    [SerializeField] SoundType slideSFX;
    [SerializeField] ParticleType slideVFX;
    [Header("Movement Settings")]
    [SerializeField] float dodgeSpeed = 5f;
    [SerializeField] float slideTime = 2f;

    void Awake()
    {
        _gravity = GetComponent<PlayerGravity>();
    }

    public void DodgeTo(float xPosition)
    {
        if (_isDodging) StopCoroutine(_moveTo);

        _moveTo = StartCoroutine(MoveTo(xPosition));
    }

    public void ApplySlide()
    {
        if (_isSliding) return;

        _slideRoutine = StartCoroutine(Slide());
    }

    public void CancelSlide()
    {
        if (!_isSliding) return;

        StopCoroutine(_slideRoutine);
        ParticleManager.Stop(slideVFX);
        SoundManager.instance.StopSound(slideSFX);
        _isSliding = false;
        transform.GetChild(0).transform.eulerAngles = Vector3.zero;
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
        ParticleManager.Play(slideVFX);
        SoundManager.instance.PlaySound(slideSFX);
        _isSliding = true;
        transform.GetChild(0).transform.eulerAngles = Vector3.right * 90f;

        yield return new WaitForSeconds(slideTime);

        ParticleManager.Stop(slideVFX);
        SoundManager.instance.StopSound(slideSFX);
        _isSliding = false;
        transform.GetChild(0).transform.eulerAngles = Vector3.zero;
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
