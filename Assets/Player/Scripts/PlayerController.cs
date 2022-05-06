using System;
using UnityEngine;

[RequireComponent(typeof(WallRun))]
[RequireComponent(typeof(PlayerGravity))]
[RequireComponent(typeof(PlayerMovement))]
[RequireComponent(typeof(PlayerCollision))]
public class PlayerController : MonoBehaviour
{
    Touch _touch;
    WallRun _wallRun;
    PlayerGravity _gravity;
    CameraFollower _camera;
    PlayerMovement _movement;

    // Middle lane is always "transform.position.x == 0f"
    int _middleLane, _currentLane, _previousLane;
    bool _isTouchEnabled = true, _isVerticalInputLocked;
    float _xOffset, _yOffset;

    public int LaneAmount { get => laneAmount; }
    public int MiddleLane { get => _middleLane; private set => _middleLane = value; }
    public int StartingLane { get => startingLane; }
    public float LaneDistance { get => laneDistance; }
    public int CurrentLane { get => _currentLane; private set => _currentLane = value; }
    public int PreviousLane { get => _previousLane; private set => _previousLane = value; }
    public float PreviousPosition { get => (PreviousLane - MiddleLane) * LaneDistance; }
    public float CurrentPosition { get => (CurrentLane - MiddleLane) * LaneDistance; }
    public bool IsVerticalInputLocked { get => _isVerticalInputLocked; set => _isVerticalInputLocked = value; }

    [SerializeField] float laneDistance = 3f;
    [SerializeField][Range(1, 7)] int laneAmount = 3;
    [SerializeField][Range(0, 6)] int startingLane = 1;

    void Awake()
    {
        _wallRun = GetComponent<WallRun>();
        _gravity = GetComponent<PlayerGravity>();
        _movement = GetComponent<PlayerMovement>();
        _camera = FindObjectOfType<CameraFollower>();
    }

    void Start()
    {
        MiddleLane = Mathf.FloorToInt(laneAmount / 2f);
        CurrentLane = Mathf.Clamp(startingLane, 0, laneAmount - 1);
        PreviousLane = CurrentLane;

        _camera.SetStartPosition(CurrentPosition);
        transform.position = new Vector3(CurrentPosition, transform.position.y, transform.position.z);
    }

    void Update()
    {
        if (!PauseGame.Instance.IsGamePaused)
        {
            ManageTouchInput();
            ManageKeyboardInput();
        }
    }

    // ------ KEYBOARD INPUT ------

    void ManageKeyboardInput()
    {
        ProcessDodgeKey();
        ProcessWallRunKey();

        if (!_movement.IsDodging && !IsVerticalInputLocked)
        {
            ProcessJumpKey();
            ProcessSlideKey();
        }
    }

    void ProcessDodgeKey()
    {
        int desiredLane = _currentLane;

        if (_wallRun.IsActivated) return;
        if (!_gravity.IsGrounded && !_gravity.IsFlying) return;

        if (Input.GetKeyDown(KeyCode.A)) desiredLane--;
        else if (Input.GetKeyDown(KeyCode.D)) desiredLane++;
        else return;

        _movement.CancelAutoSlide();
        _movement.CancelSlide();
        SwitchLane(desiredLane);
    }

    void ProcessWallRunKey()
    {
        bool isMovingToLeftWall = CurrentLane == 0 && Input.GetKeyDown(KeyCode.A);
        bool isMovingToRightWall = CurrentLane == LaneAmount - 1 && Input.GetKeyDown(KeyCode.D);

        if (_movement.IsDodging) return;
        if (_gravity.IsFlying) return;
        if (!_gravity.IsGrounded) return;

        if (isMovingToRightWall || isMovingToLeftWall) _movement.CancelSlide();

        if (isMovingToLeftWall) _wallRun.Activate(-1);
        else if (isMovingToRightWall) _wallRun.Activate(1);

    }

    void ProcessJumpKey()
    {
        if (Input.GetKeyDown(KeyCode.W))
        {
            _movement.CancelSlide();
            _gravity.ApplyJump();
        }
    }

    void ProcessSlideKey()
    {
        if (Input.GetKeyDown(KeyCode.S))
        {
            // Slide
            if (_gravity.IsGrounded && !_wallRun.IsActivated)
            {
                _movement.ApplySlide();
            }
            // Forced Fall
            else if (!_gravity.IsGrounded)
            {
                _gravity.ApplyForcedFall();
                _movement.ApplyAutoSlide();
                _wallRun.Deactivate();
            }
        }
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
                ProcessWallRunInput();

                if (!_movement.IsDodging && !IsVerticalInputLocked)
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
        if (_wallRun.IsActivated) return;
        if (!_gravity.IsGrounded && !_gravity.IsFlying) return;

        desiredLane = _xOffset >= Mathf.Epsilon ? desiredLane + 1 : desiredLane - 1;

        _movement.CancelSlide();
        _movement.CancelAutoSlide();
        SwitchLane(desiredLane);
        _isTouchEnabled = false;
    }

    void ProcessWallRunInput()
    {
        bool isMovingToLeftWall = CurrentLane == 0 && _xOffset <= -Mathf.Epsilon;
        bool isMovingToRightWall = CurrentLane == LaneAmount - 1 && _xOffset >= Mathf.Epsilon;

        if (!IsMovingHorizontally()) return;
        if (_movement.IsDodging) return;
        if (_gravity.IsFlying) return;
        if (!_gravity.IsGrounded) return;

        if (isMovingToRightWall || isMovingToLeftWall) _movement.CancelSlide();

        if (isMovingToLeftWall) _wallRun.Activate(-1);
        else if (isMovingToRightWall) _wallRun.Activate(1);

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
        if (_wallRun.IsActivated) return;
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
        _wallRun.Deactivate();

        _isTouchEnabled = false;
    }

    // Returns true if the player is able to move horizontally, or false if he's able to move vertically.
    bool IsMovingHorizontally()
    {
        float xOffset = Mathf.Abs(_xOffset);
        float yOffset = Math.Abs(_yOffset);

        return xOffset >= yOffset ? true : false;
    }

    public void SwitchLane(int desiredLane)
    {
        float nextPosition = CurrentPosition;
        desiredLane = Mathf.Clamp(desiredLane, 0, laneAmount - 1);

        if (_movement.IsDodging && _movement.IsDodgingToRight && CurrentLane < desiredLane) return;
        if (_movement.IsDodging && !_movement.IsDodgingToRight && CurrentLane > desiredLane) return;

        if (desiredLane > _currentLane) nextPosition += laneDistance;
        else if (desiredLane < _currentLane) nextPosition -= laneDistance;
        else return;

        PreviousLane = CurrentLane;
        CurrentLane = desiredLane;
        _movement.DodgeTo(nextPosition);
    }
}
