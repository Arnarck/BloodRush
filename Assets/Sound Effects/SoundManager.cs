using UnityEngine;
using System.Collections.Generic;
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

    public void PlaySound(SoundType sound, SoundCaster caster, bool isLooping)
    {
        AudioSource source = GetAvaliableSources(caster, sound, isLooping);

        source.clip = GetAudioClip(sound);
        source.loop = isLooping;
        source.Play();

        if (PauseGame.Instance.IsGamePaused && !caster.Equals(SoundCaster.Interface)) source.Pause();
    }

    public void StopSound(SoundType sound, SoundCaster caster)
    {
        List<AudioSource> sources = GetCasterSources(caster);

        foreach (AudioSource source in sources)
        {
            if (source.clip == GetAudioClip(sound))
            {
                source.Stop();
            }
        }
    }

    public void SetPauseState()
    {
        if (PauseGame.Instance.IsGamePaused)
        {
            foreach (AvaliableSource audio in avaliableSources)
            {
                if (!audio.caster.Equals(SoundCaster.Interface))
                {
                    audio.source.Pause();
                }
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

    // Return an AudioSource that is avaliable to play a clip.
    AudioSource GetAvaliableSources(SoundCaster caster, SoundType sound, bool isSoundLooping)
    {
        List<AudioSource> sources = new List<AudioSource>();
        sources = GetCasterSources(caster);

        // Search for an avaliable source.
        foreach (AudioSource source in sources)
        {
            bool isAudioClipNull = source.clip == null;
            bool isSameAudioClip = source.clip == GetAudioClip(sound);
            bool isNotPlaying = !PauseGame.Instance.IsGamePaused && !source.isPlaying;

            if (isAudioClipNull || isSameAudioClip || isNotPlaying)
            {
                return source;
            }
        }

        // If all sources are busy, an source of same loop state is searched to be replaced.
        foreach (AudioSource source in sources)
        {
            if (source.loop && isSoundLooping)
            {
                return source;
            }
        }

        // If has no avaliable sources, return the first index of the array.
        return sources[0];
    }

    // Returns all AudioSources sharing the same caster.
    List<AudioSource> GetCasterSources(SoundCaster caster)
    {
         List<AudioSource> casters = new List<AudioSource>();

        foreach (AvaliableSource avaliableSource in avaliableSources)
        {
            if (avaliableSource.caster.Equals(caster))
            {
                casters.Add(avaliableSource.source);
            }
        }

        return casters;
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
}
