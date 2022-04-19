using System;
using UnityEngine;

[RequireComponent(typeof(PlayerGravity))]
[RequireComponent(typeof(PlayerMovement))]
public class PlayerController : MonoBehaviour
{
    Touch _touch;
    PlayerGravity _gravity;
    CameraFollower _camera;
    PlayerMovement _movement;

    // Middle lane is always "transform.position.x == 0f"
    int _middleLane, _currentLane;
    bool _isTouchEnabled = true, _isVerticalInputLocked;
    float _xOffset, _yOffset;

    public int LaneAmount { get => laneAmount; }
    public int MiddleLane { get => _middleLane; }
    public int StartingLane { get => startingLane; }
    public float LaneDistance { get => laneDistance; }
    public float CurrentPosition { get => (_currentLane - _middleLane) * laneDistance; }
    public bool IsVerticalInputLocked { get => _isVerticalInputLocked; set => _isVerticalInputLocked = value; }

    [SerializeField] float laneDistance = 3f;
    [SerializeField][Range(1, 7)] int laneAmount = 3;
    [SerializeField][Range(0, 6)] int startingLane = 1;

    void Awake()
    {
        _gravity = GetComponent<PlayerGravity>();
        _movement = GetComponent<PlayerMovement>();
        _camera = FindObjectOfType<CameraFollower>();
    }

    void Start()
    {
        _middleLane = Mathf.FloorToInt(laneAmount / 2f);
        _currentLane = Mathf.Clamp(startingLane, 0, laneAmount - 1);

        _camera.SetStartPosition(CurrentPosition);
        transform.position = new Vector3(CurrentPosition, transform.position.y, transform.position.z);
    }

    void Update()
    {
        ManageTouchInput();
        ManageKeyboardInput();
    }

    // ------ KEYBOARD INPUT ------

    void ManageKeyboardInput()
    {
        if (!_movement.IsDodging && !IsVerticalInputLocked)
        {
            ProcessJumpKey();
            ProcessSlideKey();
        }
        ProcessDodgeKey();
    }

    void ProcessJumpKey()
    {
        if (Input.GetKeyDown(KeyCode.W) && _gravity.IsGrounded)
        {
            _movement.CancelSlide();
            _gravity.ApplyJump();
        }
    }

    void ProcessSlideKey()
    {
        if (Input.GetKeyDown(KeyCode.S))
        {
            if (_gravity.IsGrounded)
            {
                _movement.ApplySlide();
            }
            else
            {
                _gravity.ApplyForcedFall();
                _movement.ApplyAutoSlide();
            }
        }
    }

    void ProcessDodgeKey()
    {
        int desiredLane = _currentLane;

        if (Input.GetKeyDown(KeyCode.A)) desiredLane--;
        if (Input.GetKeyDown(KeyCode.D)) desiredLane++;
        if (desiredLane == _currentLane) return;

        _movement.CancelAutoSlide();
        _movement.CancelSlide();

        desiredLane = Mathf.Clamp(desiredLane, 0, laneAmount - 1);
        SwitchLane(desiredLane);
    }


    // ------ TOUCH INPUT ------

    void ManageTouchInput()
    {
        if (Input.touchCount < 1) return;

        _touch = Input.GetTouch(0);
        _xOffset = _touch.deltaPosition.x;
        _yOffset = _touch.deltaPosition.y;

        switch (_touch.phase)
        {
            case TouchPhase.Began:
                // do stuff
                break;

            case TouchPhase.Moved:
                ProcessDodgeInput();

                if (!_movement.IsDodging || !IsVerticalInputLocked)
                {
                    ProcessJumpInput();
                    ProcessSlideInput();
                    ProcessForcedFallInput();
                }
                break;

            case TouchPhase.Ended:
                _isTouchEnabled = true;
                break;
        }
    }

    void ProcessDodgeInput()
    {
        int desiredLane = _currentLane;

        if (!_isTouchEnabled) return;
        if (!IsMovingHorizontally()) return;

        desiredLane = _xOffset >= Mathf.Epsilon ? desiredLane + 1 : desiredLane - 1;
        desiredLane = Mathf.Clamp(desiredLane, 0, laneAmount - 1);

        _movement.CancelSlide();
        _movement.CancelAutoSlide();
        SwitchLane(desiredLane);
        _isTouchEnabled = false;
    }

    void ProcessJumpInput()
    {
        if (!_isTouchEnabled) return;
        if (IsMovingHorizontally()) return;
        if (_yOffset < Mathf.Epsilon) return;

        _movement.CancelSlide();
        _gravity.ApplyJump();
        _isTouchEnabled = false;
    }

    void ProcessSlideInput()
    {
        if (!_isTouchEnabled) return;
        if (IsMovingHorizontally()) return;
        if (!_gravity.IsGrounded) return;
        if (_yOffset >= Mathf.Epsilon) return;

        _movement.ApplySlide();
        _isTouchEnabled = false;
    }

    void ProcessForcedFallInput()
    {
        if (!_isTouchEnabled) return;
        if (IsMovingHorizontally()) return;
        if (_gravity.IsGrounded) return;
        if (_yOffset >= Mathf.Epsilon) return;

        _gravity.ApplyForcedFall();
        _movement.ApplyAutoSlide();
        _isTouchEnabled = false;
    }

    // Returns true if the player is able to move horizontally, or false if he's able to move vertically.
    bool IsMovingHorizontally()
    {
        float xOffset = Mathf.Abs(_xOffset);
        float yOffset = Math.Abs(_yOffset);

        return xOffset >= yOffset ? true : false;
    }

    void SwitchLane(int desiredLane)
    {
        float nextPosition = CurrentPosition;

        if (desiredLane > _currentLane)
        {
            nextPosition += laneDistance;
        }
        else if (desiredLane < _currentLane)
        {
            nextPosition -= laneDistance;
        }
        _currentLane = desiredLane;
        _movement.DodgeTo(nextPosition);
    }
}
