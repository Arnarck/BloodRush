using UnityEngine;
using System.Collections;

[RequireComponent (typeof(Rigidbody))]
[RequireComponent (typeof(CapsuleCollider))]
public class PlayerGravity : MonoBehaviour
{
    AudioClip _clip;
    Rigidbody _rigidBody;
    CapsuleCollider _collider;

    [Header("Sound Effects")]
    [SerializeField] AudioClip groundHitSFX;
    [SerializeField] AudioClip forcedFallSFX;

    [Header("Gravity Settings")]
    [SerializeField] LayerMask laneLayerMask;
    [SerializeField] float gravityForce = -9.81f;
    [SerializeField] float gravityScale = 1f;

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
        _rigidBody.useGravity = false;
    }

    void FixedUpdate()
    {
        ProcessGravity();
        ProcessForwardMovement();
    }

    void ProcessForwardMovement()
    {
        _rigidBody.velocity = new Vector3(_rigidBody.velocity.x, _rigidBody.velocity.y, forwardSpeed);
    }

    void ProcessGravity()
    {
        if (IsGrounded()) return;

        _rigidBody.AddForce(Vector3.up * gravityForce * gravityScale, ForceMode.Acceleration);
    }

    public void ApplyJump()
    {
        if (!IsGrounded()) return;

        _rigidBody.velocity = new Vector3(_rigidBody.velocity.x, jumpForce, _rigidBody.velocity.z);
        _clip = groundHitSFX;
        StartCoroutine(PlaySoundOnGroundCheck());
    }

    public bool IsGrounded()
    {
        Color rayColor;
        bool hasFoundColliders;
        float extraHeight = .01f;

        hasFoundColliders = Physics.Raycast(_collider.bounds.center, Vector3.down, _collider.bounds.extents.y + extraHeight, laneLayerMask);

        rayColor = hasFoundColliders ? Color.green : Color.red;
        Debug.DrawRay(_collider.bounds.center, Vector3.down * (_collider.bounds.extents.y + extraHeight), rayColor);

        return hasFoundColliders;
    }

    public void ApplyForcedFall()
    {
        // Resets the Y velocity in order to immediately starts falling
        _rigidBody.velocity = new Vector3(_rigidBody.velocity.x, 0f, _rigidBody.velocity.z);
        _rigidBody.velocity += Vector3.up * forcedFallForce;
        _clip = forcedFallSFX;
    }

    // Gambiarra
    IEnumerator PlaySoundOnGroundCheck()
    {
        yield return new WaitForSeconds(.1f);
        while (!IsGrounded())
        {
            yield return new WaitForEndOfFrame();
        }
        GetComponent<AudioSource>().PlayOneShot(_clip);
    }
}
