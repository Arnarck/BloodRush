using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(CapsuleCollider))]
public class PlayerGravity : MonoBehaviour
{
    float _currentJumpForce;
    bool _isGrounded = true, _isForcedFalling, _isFlying, _isWallRunning;

    AudioClip _clip;
    Rigidbody _rigidBody;
    CapsuleCollider _collider;

    public float ForwardSpeed { get => forwardSpeed; set => forwardSpeed = value; }
    public bool IsFlying { get => _isFlying; set => _isFlying = value; }
    public bool IsGrounded { get => _isGrounded; private set => _isGrounded = value; }
    public bool IsWallRunning { get => _isWallRunning; set => _isWallRunning = value; }
    public bool IsForcedFalling { get => _isForcedFalling; set => _isForcedFalling = value; }

    [Header("Sound Effects")]
    [SerializeField] SoundManager.SoundCaster soundCaster;
    [SerializeField] SoundType forcedFallSFX;

    [Header("Visual Effects")]
    [SerializeField] ParticleType forcedFallVFX;

    [Header("Gravity Settings")]
    [SerializeField] LayerMask laneLayerMask;
    [SerializeField] float gravityForce = -9.81f;
    [SerializeField] float gravityScale = 1f;
    [SerializeField] float raycastExtraHeight = .01f;

    [Header("Player Movement Settings")]
    [SerializeField] float jumpForce = 10f;
    [SerializeField] float forwardSpeed = 1f;
    [SerializeField] float forcedFallForce = -30f;

    void Awake()
    {
        _rigidBody = GetComponent<Rigidbody>();
        _collider = GetComponent<CapsuleCollider>();
    }

    void Start()
    {
        ResetJumpForce();
        _rigidBody.useGravity = false;
    }

    void FixedUpdate()
    {
        if (!PauseGame.Instance.IsGamePaused)
        {
            GroundCheck();
            ProcessGravity();
            ProcessForwardMovement();
        }
    }

    void ProcessForwardMovement()
    {
        _rigidBody.velocity = new Vector3(_rigidBody.velocity.x, _rigidBody.velocity.y, forwardSpeed);
    }

    void ProcessGravity()
    {
        if (IsGrounded || IsFlying || IsWallRunning) return;

        _rigidBody.AddForce(Vector3.up * gravityForce * gravityScale, ForceMode.Acceleration);
    }

    public void GroundCheck()
    {
        Color rayColor;
        bool hasFoundColliders, previousIsGrounded = IsGrounded;

        hasFoundColliders = Physics.Raycast(_collider.bounds.center, Vector3.down, _collider.bounds.extents.y + raycastExtraHeight, laneLayerMask);

        rayColor = hasFoundColliders ? Color.green : Color.red;
        Debug.DrawRay(_collider.bounds.center, Vector3.down * (_collider.bounds.extents.y + raycastExtraHeight), rayColor);

        IsGrounded = hasFoundColliders;

        if (IsGrounded && previousIsGrounded == false)
        {
            GetComponent<PlayerController>().PlayerAnimator.SetBool("isJumping", false);
            if (IsForcedFalling)
            {
                ParticleManager.Play(forcedFallVFX);
                SoundManager.instance.PlaySound(forcedFallSFX, soundCaster, false);

            }
            else
            {
                //SoundManager.instance.PlaySound(groundHitSFX, soundCaster, false);
            }
        }

        if (IsGrounded) IsForcedFalling = false;
    }

    public void ApplyJump()
    {
        if (!IsGrounded) return;

        GetComponent<PlayerController>().PlayerAnimator.SetBool("isJumping", true);
        _rigidBody.velocity = new Vector3(_rigidBody.velocity.x, _currentJumpForce, _rigidBody.velocity.z);
    }

    public void ApplyForcedFall()
    {
        // Resets the Y velocity in order to immediately starts falling
        _rigidBody.velocity = new Vector3(_rigidBody.velocity.x, 0f, _rigidBody.velocity.z);
        _rigidBody.velocity += Vector3.up * forcedFallForce;
        IsForcedFalling = true;
    }

    public void SetJumpForce(float force)
    {
        _currentJumpForce = force;
    }

    public void ResetJumpForce()
    {
        _currentJumpForce = jumpForce;
    }
}
