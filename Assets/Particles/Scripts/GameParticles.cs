using UnityEngine;

public class GameParticles : MonoBehaviour
{
    public static GameParticles Instance;

    public ParticleEffect[] particles;

    [System.Serializable]
    public  class ParticleEffect
    {
        public ParticleType name;
        public ParticleSystem vfx;
    }

    void Start()
    {
        Instance = this;
    }
}
