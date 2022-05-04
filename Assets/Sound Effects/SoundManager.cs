using UnityEngine;
using UnityEngine.UI;

public class SoundManager: MonoBehaviour
{
    public static SoundManager instance;

    public enum SoundCaster
    {
        Player,
        Collectable,
        Interface,
        Music
    }

    [System.Serializable]
    class AvaliableSource
    {
        public SoundCaster caster;
        public AudioSource source;
    }

    [SerializeField] Slider sfxSlider;
    [SerializeField] AvaliableSource[] avaliableSources;

    void Start()
    {
        instance = this; 
    }

    // Gets the specified sound from the pool. If it not exists, a AudioSource is removed to host the clip.
    public void PlaySound(SoundType sound, SoundCaster caster)
    {
        AudioSource source = GetAudioSource(caster);
        source.clip = GetAudioClip(sound);
        source.Play();
        if (PauseGame.Instance.IsGamePaused) source.Pause();
    }

    public void StopSound(SoundType sound, SoundCaster caster)
    {
        AudioSource source = GetAudioSource(caster);
        source.Stop();
    }

    AudioSource GetAudioSource(SoundCaster caster)
    {
        foreach (AvaliableSource source in avaliableSources)
        {
            if (caster.Equals(SoundCaster.Player))
            {
                // search for an source that is not playing
                // if not found (SHOULD NOT HAPPEN), take the last one
            }

            if (source.caster.Equals(caster))
            {
                return source.source;
            }
        }

        Debug.LogError("Sound Caster" + caster + " not found!");
        return null;
    }

    AudioClip GetAudioClip(SoundType sound)
    {
        foreach (GameSounds.SoundAudioClip audioClip in GameSounds.instance.sounds)
        {
            if (audioClip.name.Equals(sound))
            {
                return audioClip.clip;
            }
        }

        Debug.LogError("Sound clip" + sound + " not found!");
        return null;
    }

    public void SetPauseState()
    {
        if (PauseGame.Instance.IsGamePaused)
        {
            foreach (AvaliableSource audio in avaliableSources)
            {
                audio.source.Pause();
            }
        }
        else
        {
            foreach (AvaliableSource audio in avaliableSources)
            {
                audio.source.UnPause();
            }
        }
    }

    public void ModifyVolume()
    {
        foreach (AvaliableSource audio in avaliableSources)
        {
            audio.source.volume = sfxSlider.value;
        }
    }
}
