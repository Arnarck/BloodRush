using System.Collections.Generic;
using UnityEngine;

public class SoundPool : MonoBehaviour
{
    List<AudioSource> _audioSources;

    public List<AudioSource> AudioSources { get => _audioSources; }

    [SerializeField] int poolSize;

    void Start()
    {
        GeneratePool(poolSize);
    }

    void GeneratePool(int size)
    {
        _audioSources = new List<AudioSource>();
        for (int i = 0; i < poolSize; i++)
        {
            AddToPool();
        }
    }

    AudioSource AddToPool()
    {
        GameObject go = new GameObject("Sound");
        AudioSource source;
        go.transform.SetParent(transform);

        source = go.AddComponent<AudioSource>();
        _audioSources.Add(source);
        source.playOnAwake = false;
        return source;
    }

    public AudioClip GetAudioClip(SoundType sound)
    {
        foreach (GameSounds.SoundAudioClip soundAudioClip in GameSounds.instance.sounds)
        {
            if (soundAudioClip.name.Equals(sound))
            {
                return soundAudioClip.clip;
            }
        }

        Debug.LogError("Sound " + sound + " not found!");
        return default;
    }

    public AudioSource RemoveFromPool()
    {
        foreach (AudioSource source in _audioSources)
        {
            //if ( (!source.isPlaying && !PauseGame.Instance.IsGamePaused) || source.clip == null)
            //{
            //    return source;
            //}
            if (source.clip == null) return source;
        }

        return AddToPool();
    }

    public AudioSource GetAudioSource(SoundType sound)
    {
        AudioClip clip = GetAudioClip(sound);
        foreach (AudioSource source in _audioSources)
        {
            if (source.clip == clip) return source;
        }

        return default;
    }
}
