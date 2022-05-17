using System.Collections;
using UnityEngine;

public class Fly : Powerup
{
    Rigidbody _rigidBody;
    PlayerGravity _gravity;
    CameraFollower _camera;
    PlayerMovement _movement;
    Coroutine _disableFlyCamera;
    PlayerController _controller;

    bool _isTravelling;

    public bool IsTravelling { get => _isTravelling; set => _isTravelling = value; }

    [SerializeField] ParticleType batTransformationVFX;
    [SerializeField] float cameraSpeed = 5f;
    [SerializeField] float flyPosition = 6f;
    [SerializeField] float travelSpeed = 5f;

    void Awake()
    {
        _rigidBody = GetComponent<Rigidbody>();
        _gravity = GetComponent<PlayerGravity>();
        _movement = GetComponent<PlayerMovement>();
        _camera = FindObjectOfType<CameraFollower>();
        _controller = GetComponent<PlayerController>();
    }

    public override void Activate()
    {
        if (IsActivated)
        {
            StopCoroutine(CountdownRoutine);
            CountdownRoutine = StartCoroutine(CountdownToDeactivate());
            return;
        }

        if (_disableFlyCamera != null)
        {
            StopCoroutine(_disableFlyCamera);
            _camera.IsFallingAfterFly = false;
        }

        ParticleManager.Play(batTransformationVFX);
        IsActivated = true;
        _camera.IsFlying = true;
        _gravity.IsFlying = true;
        _gravity.IsForcedFalling = false;
        _controller.IsVerticalInputLocked = true;

        StopPlayerMovement();
        StartCoroutine(TravelToFlyCoordinates());
        _camera.TravelTo(flyPosition, cameraSpeed);

        CountdownRoutine = StartCoroutine(CountdownToDeactivate());
    }

    void StopPlayerMovement()
    {
        _rigidBody.velocity = new Vector3(_rigidBody.velocity.x, 0f, _rigidBody.velocity.z);

        _movement.CancelSlide();
        _movement.CancelAutoSlide();
    }

    IEnumerator TravelToFlyCoordinates()
    {
        float yStart = transform.position.y;
        float movementPercentage = 0f;

        IsTravelling = true;
        while (movementPercentage < 1f)
        {
            float yPosition;
            float movementThisFrame = Time.deltaTime * travelSpeed;

            movementPercentage += movementThisFrame;
            yPosition = Mathf.Lerp(yStart, flyPosition, movementPercentage);
            transform.position = new Vector3(transform.position.x, yPosition, transform.position.z);

            yield return new WaitForEndOfFrame();
        }
        IsTravelling = false;
    }

    protected override IEnumerator CountdownToDeactivate()
    {
        while (IsTravelling)
        {
            yield return new WaitForEndOfFrame();
        }

        yield return new WaitForSeconds(CurrentLifetime);
        Deactivate();
    }

    protected override void Deactivate()
    {
        IsActivated = false;
        _camera.IsFlying = false;
        _gravity.IsFlying = false;
        _controller.IsVerticalInputLocked = false;

        _disableFlyCamera = StartCoroutine(DisableFlyCamera());
    }

    IEnumerator DisableFlyCamera()
    {
        _camera.IsFallingAfterFly = true;
        while (!_gravity.IsGrounded)
        {
            yield return new WaitForEndOfFrame();
        }
        _camera.IsFallingAfterFly = false;
    }
}
