using UnityEngine;

[RequireComponent(typeof(BerserkerBar))]
public class PlayerCollision : MonoBehaviour
{
    bool _isInvincible;

    Fly _flyPowerup;
    HigherJump _jumpPowerup;
    BerserkerBar _berserkerBar;

    public bool IsInvincible { get => _isInvincible; private set => _isInvincible = value; }

    [SerializeField] GameObject bloodSplashVFX;
    [SerializeField] SoundType hit;

    void Awake()
    {
        _flyPowerup = GetComponent<Fly>();
        _jumpPowerup = GetComponent<HigherJump>();
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
            case "Coin":
                // Increase coin count
                // Coins must have a property for how much they fill the berserker bar
                _berserkerBar.ModifyCurrentValue(1);
                break;

            case "Obstacle":
                if (IsInvincible) return;

                Instantiate(bloodSplashVFX, transform.position, transform.rotation);
                SoundManager.instance.PlaySound(hit);
                if (_berserkerBar.CurrentValue == 0)
                {
                    GameOver.Instance.Activate();
                }
                else
                {
                    _berserkerBar.CurrentValue = 0;
                }
                // If player is transformed, just reduce the bar
                // If the bar is already empty, kill the player
                // Decreases player speed?
                break;

            case "Lethal":
                _flyPowerup.Activate();
                //_jumpPowerup.Activate();
                // Kill player
                // Start Game Over process (enable game over screen, stop the game, deposit coins, etc.)
                break;

            case "Finish":
                _jumpPowerup.Activate();
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
