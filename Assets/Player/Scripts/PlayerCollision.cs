using UnityEngine;

[RequireComponent(typeof(BerserkerBar))]
public class PlayerCollision : MonoBehaviour
{
    bool _isInvincible;

    Fly _flyPowerup;
    HigherJump _jumpPowerup;
    BerserkerBar _berserkerBar;
    PlayerMovement _movement;

    public bool IsInvincible { get => _isInvincible; private set => _isInvincible = value; }

    [Header("Sound Effects")]
    [SerializeField] SoundManager.SoundCaster playerCaster;
    [SerializeField] SoundManager.SoundCaster collectableCaster;
    [SerializeField] SoundType hitSFX;

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
                // Increase coin count
                // Coins must have a property for how much they fill the berserker bar
                ParticleManager.Play(collectableVFX);
                _berserkerBar.ModifyCurrentValue(1);
                break;

            case "Obstacle":
                if (IsInvincible) return;

                ParticleManager.Play(hitVFX);
                SoundManager.instance.PlaySound(hitSFX, playerCaster, false);
                if (_berserkerBar.CurrentValue == 0)
                {
                    GameOver.Instance.Activate();
                }
                else
                {
                    _berserkerBar.CurrentValue = 0;
                    _movement.MoveToPreviousLane();
                }
                // If player is transformed, just reduce the bar
                // If the bar is already empty, kill the player
                // Decreases player speed?
                break;

            case "Lethal":
                GameOver.Instance.Activate();
                // Start Game Over process (enable game over screen, stop the game, deposit coins, etc.)
                break;

            case "Fly":
                _flyPowerup.Activate();
                ParticleManager.Play(powerupCollectedVFX);
                break;

            case "ScoreMultiplier":
                // Active score multiplier
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

    public void ToggleInvincibility()
    {
        IsInvincible = !_isInvincible;
    }
}
