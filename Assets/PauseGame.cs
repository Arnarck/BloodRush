using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PauseGame : MonoBehaviour
{
    bool _isGamePaused;
    public static PauseGame Instance;

    public bool IsGamePaused {  get => _isGamePaused; private set { _isGamePaused = value; } }

    [SerializeField] GameObject pauseScreen;

    // Start is called before the first frame update
    void Start()
    {
        Instance = this;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            SetPause(!IsGamePaused);
        }
    }

    public void SetPause(bool isPaused)
    {
        SetGameFlow(isPaused);
        pauseScreen.SetActive(IsGamePaused);
    }

    public void SetGameFlow(bool isPaused)
    {
        IsGamePaused = isPaused;
        SoundManager.instance.SetPauseState();
        Time.timeScale = IsGamePaused ? 0f : 1f;
    }
}
