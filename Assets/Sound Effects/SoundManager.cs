using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(SoundPool))]
[RequireComponent(typeof(GameSounds))]
public class SoundManager: MonoBehaviour
{
    SoundPool _soundPool;
    public static SoundManager instance;

    [SerializeField] Slider sfxSlider;

    void Awake()
    {
        _soundPool = GetComponent<SoundPool>();
    }

    void Start()
    {
        instance = this; 
    }

    // Gets the specified sound from the pool. If it not exists, a AudioSource is removed to host the clip.
    public void PlaySound(SoundType sound)
    {
        AudioSource source = _soundPool.GetAudioSource(sound);
        if (source == null)
        {
            source = _soundPool.RemoveFromPool();
            source.clip = _soundPool.GetAudioClip(sound);
        }
        if (PauseGame.Instance.IsGamePaused && sound == SoundType.PlayerSlide) Debug.Log("yo");

        source.Play();
        if (PauseGame.Instance.IsGamePaused) source.Pause();
    }

    public void StopSound(SoundType sound)
    {
        AudioSource source = _soundPool.GetAudioSource(sound);
        source.Stop();
    }

    public void SetPauseState()
    {
        if (PauseGame.Instance.IsGamePaused)
        {
            foreach (AudioSource source in _soundPool.AudioSources)
            {
                source.Pause();
            }
        }
        else
        {
            foreach (AudioSource source in _soundPool.AudioSources)
            {
                source.UnPause();
            }
        }
    }

    public void ModifyVolume()
    {
        foreach (AudioSource source in _soundPool.AudioSources)
        {
            source.volume = sfxSlider.value;
        }
    }
}
