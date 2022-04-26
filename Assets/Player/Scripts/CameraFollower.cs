using UnityEngine;
using System.Collections;

public class CameraFollower : MonoBehaviour
{
    bool _isFallingAfterFly, _isFlying;
    float _xOffset, _yOffset, _currentJumpOffset;
    Vector3 _startPos, _currentPos, _previousPos;

    Transform _player;
    PlayerGravity _gravity;

    public bool IsFlying { get => _isFlying; set => _isFlying = value; }
    public bool IsFallingAfterFly { get => _isFallingAfterFly; set => _isFallingAfterFly = value; }

    [Header("Basic Movement Settings")]
    [SerializeField][Range(0f, 1f)] float dodgeOffset;
    [SerializeField][Range(0f, 1f)] float jumpOffset;

    void Awake()
    {
        _player = GameObject.FindWithTag("Player").transform;

        _gravity = _player.GetComponent<PlayerGravity>();
    }

    void Start()
    {
        _startPos = transform.position;
        _currentPos = _player.position;
        _previousPos = _player.position;

        _xOffset = _startPos.x;
        _yOffset = _startPos.y;

        ResetJumpOffset();
    }

    void LateUpdate()
    {
        _currentPos = _player.position;

        ProcessHorizontalMovement();
        ProcessVerticalMovement();
        transform.position = new Vector3(_xOffset, _yOffset, _player.position.z);

        _previousPos = _currentPos;
    }

    public void SetStartPosition(float xPlayerPosition)
    {
        float distanceToMove = xPlayerPosition * dodgeOffset;

        transform.position = new Vector3(distanceToMove, transform.position.y, _player.position.z);
    }

    void ProcessHorizontalMovement()
    {
        float rawOffset = _currentPos.x - _previousPos.x;
        _xOffset = transform.position.x + (rawOffset * dodgeOffset);
    }

    void ProcessVerticalMovement()
    {
        if (IsFlying)
        {
            _yOffset = transform.position.y;
            return;
        }

        float rawOffset = _currentPos.y - _previousPos.y;

        if (_gravity.IsGrounded)
        {
            _yOffset = _startPos.y;
            return;
        }

        if (IsFallingAfterFly)
        {
            _yOffset = _player.position.y + 2.1f;
        }
        else
        {
            _yOffset = transform.position.y + (rawOffset * _currentJumpOffset);
        }
    }

    public void TravelTo(float yPosition, float speed)
    {
        float flyPosition = yPosition + 2.1f;
        StartCoroutine(FlyTo(flyPosition, speed));
    }

    IEnumerator FlyTo(float yEnd, float flySpeed)
    {
        float yStart = transform.position.y;
        float movementPercentage = 0f;

        while (movementPercentage < 1f)
        {
            float yPosition;
            float movementThisFrame = Time.deltaTime * flySpeed;

            movementPercentage += movementThisFrame;
            yPosition = Mathf.Lerp(yStart, yEnd, movementPercentage);
            transform.position = new Vector3(transform.position.x, yPosition, transform.position.z);

            yield return new WaitForEndOfFrame();
        }
    }

    public void SetJumpOffset(float offset)
    {
        _currentJumpOffset = offset;
    }

    public void ResetJumpOffset()
    {
        _currentJumpOffset = jumpOffset;
    }
}
