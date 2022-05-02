using UnityEngine;

public class GameOver : MonoBehaviour
{
    public static GameOver Instance { get; private set; }

    [SerializeField] GameObject gameOverScreen;

    void Start()
    {
        Instance = this;
    }

    public void Activate()
    {
        PauseGame.Instance.SetPause(true);
        gameOverScreen.SetActive(true);
    }
}
