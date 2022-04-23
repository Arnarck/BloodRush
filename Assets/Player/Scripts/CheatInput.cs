using UnityEngine;
using TMPro;

public class CheatInput : MonoBehaviour
{
    PlayerCollision _collision;

    [SerializeField] int invincibilityTouchCount = 5;
    [SerializeField] TextMeshProUGUI invincibilityMessage;

    // Start is called before the first frame update
    void Awake()
    {
        if (!Debug.isDebugBuild) Destroy(this);

        _collision = GetComponent<PlayerCollision>();
    }

    // Update is called once per frame
    void Update()
    {
        ManageKeyboardInput();
        ManageTouchInput();
    }

    // ================ KEYBOARD INPUT ================

    void ManageKeyboardInput()
    {
        ProcessInvincibilityKey();
    }

    void ProcessInvincibilityKey()
    {
        if (Input.GetKeyDown(KeyCode.A) && Input.GetKeyDown(KeyCode.D))
        {
            ToggleInvincibility();
        }
    }

    // ================ TOUCH INPUT ================

    void ManageTouchInput()
    {
        ProcessInvincibilityTouch();
    }

    void ProcessInvincibilityTouch()
    {
        if (!IsCheatTouchPressed(invincibilityTouchCount)) return;

        ToggleInvincibility();
    }

    bool IsCheatTouchPressed(int touchCount)
    {
        if (Input.touchCount != touchCount) return false;
        foreach (Touch touch in Input.touches)
        {
            if (touch.phase != TouchPhase.Began) return false;
        }
        return true;
    }

    void ToggleInvincibility()
    {
        _collision.ToggleInvincibility();
        invincibilityMessage.gameObject.SetActive(_collision.IsInvincible);
    }
}
