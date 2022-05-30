using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuNavigation : MonoBehaviour
{
    [SerializeField] SoundType buttonSFX;
    [SerializeField] SoundManager.SoundCaster caster;

    public void ToggleScreenActive(GameObject screen)
    {
        screen.SetActive(!screen.activeInHierarchy);
        SoundManager.instance.PlaySound(buttonSFX, caster, false);
    }

    public void StartGame()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(1);
        SoundManager.instance.PlaySound(buttonSFX, caster, false);
    }

    public void BackToMainMenu()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(0);
        SoundManager.instance.PlaySound(buttonSFX, caster, false);
    }

    public void QuitGame()
    {
        Application.Quit();
        SoundManager.instance.PlaySound(buttonSFX, caster, false);
    }
}
