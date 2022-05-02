using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameSounds : MonoBehaviour
{
    public static GameSounds instance;

    public SoundAudioClip[] sounds;

    void Start()
    {
        instance = this;
    }
    
    [System.Serializable]
    public class SoundAudioClip
    {
        public SoundType name;
        public AudioClip clip;
    }
}
