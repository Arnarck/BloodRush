using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CapsuleCollider))]
public class WallRun : MonoBehaviour
{
    ParticleType _currentSparksVFX;
    bool _isActivated, _isTravelling;
    int _currentWallDirection;

    PlayerGravity _gravity;
    CapsuleCollider _collider;

    public bool IsActivated { get => _isActivated; private set => _isActivated = value; }
    public bool IsTravelling { get => _isTravelling; private set => _isTravelling = value; }

    [Header("Effects")]
    [SerializeField] ParticleType LeftSparksVFX;
    [SerializeField] ParticleType RightSparksVFX;
    [SerializeField] SoundType wallrunSFX;
    [SerializeField] SoundManager.SoundCaster caster;
    [Header("Wall Run Settings")]
    [SerializeField] LayerMask wallrunLayerMask;
    [SerializeField] float yWallRunPosition;
    [SerializeField] float rayOffset = 1f;
    [SerializeField] float lifeTime = 5f;
    [SerializeField] float travelSpeed = 5f;

    void Awake()
    {
        _gravity = GetComponent<PlayerGravity>();
        _collider = GetComponent<CapsuleCollider>();
    }

    public void Activate(int wallDirection)
    {
        // Normalize "wallDirection" value
        wallDirection = wallDirection / Mathf.Abs(wallDirection);

        if (IsActivated) return;
        if (!IsWallExists(wallDirection)) return;

        _currentSparksVFX = wallDirection > 0 ? RightSparksVFX : LeftSparksVFX;
        _currentWallDirection = wallDirection;

        SetActiveState(true);
        StartCoroutine(TravelTo(yWallRunPosition));
        StartCoroutine(CountdownToDeactivate());
        StartCoroutine(WallCheck(wallDirection));
    }

    bool IsWallExists(int direction)
    {
        Color rayColor;
        Vector3 origin = _collider.bounds.center + (Vector3.forward * _collider.bounds.extents.z);
        RaycastHit hitInfo;
        bool hasFoundColliders;

        hasFoundColliders = Physics.Raycast(origin, Vector3.right * direction, out hitInfo, rayOffset, wallrunLayerMask);
        rayColor = hasFoundColliders ? Color.green : Color.red;
        Debug.DrawRay(origin, Vector3.right * direction * rayOffset, rayColor);

        return hasFoundColliders;
    }

    IEnumerator WallCheck(int wallDirection)
    {
        while (IsWallExists(wallDirection))
        {
            yield return new WaitForEndOfFrame();
        }

        Deactivate();
    }

    IEnumerator TravelTo(float yEnd)
    {
        float yStart = transform.position.y;
        float travelPercentage = 0f;

        IsTravelling = true;
        while (travelPercentage < 1f)
        {
            float yThisFrame;
            float travelThisFrame = Time.deltaTime * travelSpeed;

            travelPercentage += travelThisFrame;
            yThisFrame = Mathf.Lerp(yStart, yEnd, travelPercentage);
            transform.position = new Vector3(transform.position.x, yThisFrame, transform.position.z);

            yield return new WaitForEndOfFrame();
        }
        IsTravelling = false;
    }

    IEnumerator CountdownToDeactivate()
    {
        while (IsTravelling)
        {
            yield return new WaitForEndOfFrame();
        }

        yield return new WaitForSeconds(lifeTime);
        Deactivate();
    }

    public void Deactivate()
    {
        StopAllCoroutines();

        SetActiveState(false);
    }

    public void SetActiveState(bool isActivated)
    {
        IsActivated = isActivated;
        _gravity.IsWallRunning = isActivated;

        if (isActivated)
        {
            transform.GetChild(0).transform.rotation = Quaternion.Euler(0f, 0f, 15f * _currentWallDirection);
            ParticleManager.Play(_currentSparksVFX);
            SoundManager.instance.PlaySound(wallrunSFX, caster, true);
        }
        else
        {
            transform.GetChild(0).transform.rotation = Quaternion.identity;
            ParticleManager.Stop(_currentSparksVFX);
            SoundManager.instance.StopSound(wallrunSFX, caster);
        }
    }
}
