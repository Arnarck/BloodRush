using UnityEngine;

[RequireComponent(typeof(BerserkerBar))]
public class PlayerCollision : MonoBehaviour
{
    [SerializeField] GameObject bloodSplashVFX;
    BerserkerBar _berserkerBar;

    // Start is called before the first frame update
    void Start()
    {
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
                Instantiate(bloodSplashVFX, transform.position, transform.rotation);
                if (_berserkerBar.CurrentValue == 0)
                {
                    Debug.Log("died");
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
                // Kill player
                // Start Game Over process (enable game over screen, stop the game, deposit coins, etc.)
                break;

            default:
                break;
        }
    }
}
