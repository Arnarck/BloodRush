using UnityEngine;
using TMPro;

[RequireComponent(typeof(BerserkerBar))]
public class PlayerCollision : MonoBehaviour
{
    bool _isInvincible;
    int _bloodCollected;

    Fly _flyPowerup;
    HigherJump _jumpPowerup;
    PlayerMovement _movement;
    BerserkerBar _berserkerBar;
    BerserkerMode _berserkerMode;
    ScoreMultiplier _scorePowerup;

    public bool IsInvincible { get => _isInvincible; private set => _isInvincible = value; }
    public int BloodCollected { get => _bloodCollected; }

    [SerializeField] int transformedObstacleDamage = 2;
    [SerializeField] int transformedLethalDamage = 5;
    [SerializeField] TextMeshProUGUI bloodDisplay;
    [Header("Sound Effects")]
    [SerializeField] SoundManager.SoundCaster playerCaster;
    [SerializeField] SoundManager.SoundCaster collectableCaster;
    [SerializeField] SoundType hitSFX;
    [SerializeField] SoundType collectSFX;

    [Header("Visual Effects")]
    [SerializeField] ParticleType hitVFX;
    [SerializeField] ParticleType collectableVFX;
    [SerializeField] ParticleType powerupCollectedVFX;

    void Awake()
    {
        _flyPowerup = GetComponent<Fly>();
        _jumpPowerup = GetComponent<HigherJump>();
        _movement = GetComponent<PlayerMovement>();
        _berserkerBar = GetComponent<BerserkerBar>();
        _berserkerMode = FindObjectOfType<BerserkerMode>();
        _scorePowerup = GetComponent<ScoreMultiplier>();
    }

    void Start()
    {
        bloodDisplay.text = _bloodCollected.ToString();
    }

    void OnTriggerEnter(Collider other)
    {
        ProcessCollision(other.tag);
    }

    void ProcessCollision(string other)
    {
        switch (other)
        {
            case "Collectable":
                CollectBlood();
                break;

            case "Obstacle":
                if (IsInvincible) return;

                ParticleManager.Play(hitVFX);
                SoundManager.instance.PlaySound(hitSFX, playerCaster, false);

                if (_berserkerMode.IsTransformed)
                {
                    _berserkerMode.ReduceBarValue(transformedObstacleDamage);
                    return;
                }

                if (_berserkerBar.CurrentValue == 0)
                {
                    SaveData.ModifyBloodAmount(_bloodCollected);
                    GameOver.Instance.Activate();
                }
                else
                {
                    _berserkerBar.CurrentValue = 0;
                    _movement.MoveToPreviousLane();
                }
                break;

            case "Lethal":
                if (IsInvincible) return;

                if (_berserkerMode.IsTransformed)
                {
                    ParticleManager.Play(hitVFX);
                    SoundManager.instance.PlaySound(hitSFX, playerCaster, false);
                    _berserkerMode.ReduceBarValue(transformedLethalDamage);
                    _movement.MoveToPreviousLane();
                    return;
                }

                SaveData.ModifyBloodAmount(_bloodCollected);
                GameOver.Instance.Activate();
                break;

            case "Fly":
                _flyPowerup.Activate();
                ParticleManager.Play(powerupCollectedVFX);
                break;

            case "ScoreMultiplier":
                _scorePowerup.Activate();
                ParticleManager.Play(powerupCollectedVFX);
                break;

            case "HigherJump":
                _jumpPowerup.Activate();
                ParticleManager.Play(powerupCollectedVFX);
                break;

            default:
                break;
        }
    }

    void CollectBlood()
    {
        _bloodCollected++;
        bloodDisplay.text = _bloodCollected.ToString();
        ParticleManager.Play(collectableVFX);
        SoundManager.instance.PlaySound(collectSFX, collectableCaster, false);
        _berserkerBar.ModifyCurrentValue(1);
    }

    public void ToggleInvincibility()
    {
        IsInvincible = !_isInvincible;
    }
}
