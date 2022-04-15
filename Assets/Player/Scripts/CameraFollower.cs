using UnityEngine;

public class CameraFollower : MonoBehaviour
{
    float xStart, xEnd, xMovement;
    Vector3 currentPos, previousPos;

    PlayerController _player;

    [Header("Dodge Settings")]
    [SerializeField] float dodgeSpeed = 5f;
    [SerializeField] [Range(0f, 1f)]float dodgeOffset;

    [Header("Jump Settings")]
    [SerializeField] float jumpPosition = 4f;
    [SerializeField][Range(0f, 1f)] float jumpOffset;

    [Header("Dash Settings")]
    [SerializeField] float dashPosition = 2f;

    void Awake()
    {
        _player = GameObject.FindWithTag("Player").GetComponent<PlayerController>();
    }

    void Start()
    {
        currentPos = _player.transform.position;
        previousPos = _player.transform.position;
    }

    void LateUpdate()
    {
        currentPos = _player.transform.position;
        float x = ProcessDodgeMovement();
        float y = ProcessJumpMovement();
        transform.position = new Vector3(x, y, _player.transform.position.z);
        previousPos = _player.transform.position;
    }

    float ProcessDodgeMovement()
    {
        xMovement += dodgeSpeed * Time.deltaTime;
        return Mathf.Lerp(xStart, xEnd, xMovement);
    }

    float ProcessJumpMovement()
    {
        //if (FindObjectOfType<PlayerGravity>().IsGrounded()) return yPivot;

        float offset = currentPos.y - previousPos.y;
        return transform.position.y + (offset * jumpOffset);
    }

    public void SetStartPosition(float xPosition)
    {
        float distanceToMove = xPosition * dodgeOffset;

        transform.position = new Vector3(distanceToMove, transform.position.y, _player.transform.position.z);
    }

    public void StartDodgeMovement()
    {
        xMovement = 0f;
        xStart = transform.position.x;
        xEnd = _player.CurrentPosition * dodgeOffset;
    }
}
