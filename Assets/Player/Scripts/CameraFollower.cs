using UnityEngine;
using System.Collections;

public class CameraFollower : MonoBehaviour
{
    bool _isFallingAfterFly, _isFlying;
    float _yOffset, _currentJumpOffset;
    Vector3 _startPos, _playerCurrentPos, _playerPreviousPos;

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
        _playerCurrentPos = _player.position;
        _playerPreviousPos = _player.position;
        _yOffset = transform.position.y - _player.position.y;

        ResetJumpOffset();
    }

    void LateUpdate()
    {
        if (!PauseGame.Instance.IsGamePaused)
        {
            float xThisFrame, yThisFrame;

            _playerCurrentPos = _player.position;
            xThisFrame = ProcessHorizontalMovement();
            yThisFrame = ProcessVerticalMovement();

            transform.position = new Vector3(xThisFrame, yThisFrame, _player.position.z);

            _playerPreviousPos = _playerCurrentPos;
        }
    }

    public void SetStartPosition(float xPlayerPosition)
    {
        float distanceToMove = xPlayerPosition * dodgeOffset;

        transform.position = new Vector3(distanceToMove, transform.position.y, _player.position.z);
    }

    float ProcessHorizontalMovement()
    {
        float rawOffset = _playerCurrentPos.x - _playerPreviousPos.x;
        return transform.position.x + (rawOffset * dodgeOffset);
    }

    float ProcessVerticalMovement()
    {
        float rawOffset = _playerCurrentPos.y - _playerPreviousPos.y;

        if (IsFlying) return transform.position.y;

        if (_gravity.IsGrounded) return _startPos.y;

        if (IsFallingAfterFly) return _player.position.y + _yOffset;
        else return transform.position.y + (rawOffset * _currentJumpOffset);
    }

    public void TravelTo(float yPosition, float speed)
    {
        float flyPosition = yPosition + _yOffset;
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
