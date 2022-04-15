﻿using System.Collections;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    PlayerGravity _gravity;
    Coroutine _dodgeRoutine, _slideRoutine, _autoSlideRoutine;
    bool _isDodging, _isSliding, _isAutoSlideEnabled;

    public bool IsDodging { get => _isDodging; }

    [SerializeField] GameObject forcedFallVFX;
    [SerializeField] ParticleSystem slideVFX;
    [SerializeField] float dodgeSpeed = 5f;
    [SerializeField] float slideTime = 2f;

    void Awake()
    {
        _gravity = GetComponent<PlayerGravity>();
    }

    void Start()
    {
        slideVFX.Stop();
    }

    public void DodgeTo(float xPosition)
    {
        if (_isDodging) StopCoroutine(_dodgeRoutine);

        _dodgeRoutine = StartCoroutine(MoveTo(xPosition));
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
        slideVFX.Stop();
        slideVFX.GetComponent<AudioSource>().Stop();
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
        slideVFX.Play();
        slideVFX.GetComponent<AudioSource>().Play();
        _isSliding = true;
        transform.GetChild(0).transform.eulerAngles = Vector3.right * 90f;

        yield return new WaitForSeconds(slideTime);

        slideVFX.Stop();
        slideVFX.GetComponent<AudioSource>().Stop();
        _isSliding = false;
        transform.GetChild(0).transform.eulerAngles = Vector3.zero;
    }

    IEnumerator AutoSlide()
    {
        _isAutoSlideEnabled = true;
        while (!_gravity.IsGrounded())
        {
            yield return new WaitForEndOfFrame();
        }
        _isAutoSlideEnabled = false;

        Instantiate(forcedFallVFX, new Vector3(transform.position.x, 0f, transform.position.z), transform.rotation);
        _slideRoutine = StartCoroutine(Slide());
    }

    IEnumerator MoveTo(float endPosition)
    {
        float startPosition = transform.position.x;
        float movementPercentage = 0f;

        _isDodging = true;
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
