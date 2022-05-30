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

    float _musicVolume;
    float _pausedMusicVolume;

    [SerializeField] Slider sfxSlider;
    [SerializeField] Slider musicSlider;
    [SerializeField] SoundType music;
    [SerializeField] SoundCaster musicCaster;
    [SerializeField][Range(0f, 1f)] float pausedMusicVolume = .25f;
    [SerializeField] AvaliableSource[] avaliableSources;

    void Start()
    {
        _pausedMusicVolume = pausedMusicVolume;
        instance = this;
        PlaySound(music, musicCaster, true);
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
                if (!audio.caster.Equals(SoundCaster.Interface) && !audio.caster.Equals(SoundCaster.Music))
                {
                    audio.source.Pause();
                }

                if (audio.caster.Equals(SoundCaster.Music))
                {
                    _musicVolume = audio.source.volume;
                    audio.source.volume = _pausedMusicVolume;
                }
            }
        }
        else
        {
            foreach (AvaliableSource audio in avaliableSources)
            {
                if (!audio.caster.Equals(SoundCaster.Interface) && !audio.caster.Equals(SoundCaster.Music))
                {
                    audio.source.UnPause();
                }

                if (audio.caster.Equals(SoundCaster.Music))
                {
                    audio.source.volume = _musicVolume;
                }
            }
        }
    }

    public void ModifySFXVolume()
    {
        foreach (AvaliableSource audio in avaliableSources)
        {
            if (!audio.caster.Equals(SoundCaster.Music)) audio.source.volume = sfxSlider.value;
        }
    }

    public void ModifyMusicVolume()
    {
        foreach (AvaliableSource audio in avaliableSources)
        {
            if (audio.caster.Equals(SoundCaster.Music))
            {
                _musicVolume = musicSlider.value;
                _pausedMusicVolume = pausedMusicVolume * musicSlider.value;
                audio.source.volume = _pausedMusicVolume;
            }
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
