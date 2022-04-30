using UnityEngine;

public static class ParticleManager
{
    public static void Play(ParticleType particle)
    {
        ParticleSystem vfx = GetParticleEffect(particle);
        if (vfx != null)
        {
            vfx.gameObject.SetActive(true);
            vfx.Play();
        }
    }

    public static void Stop(ParticleType particle)
    {
        ParticleSystem vfx = GetParticleEffect(particle);
        if (vfx != null)
        {
            vfx.Stop();
        }
    }

    static ParticleSystem GetParticleEffect(ParticleType particle)
    {
        foreach (GameParticles.ParticleEffect effect in GameParticles.Instance.particles)
        {
            if (effect.name.Equals(particle))
            {
                return effect.vfx;
            }
        }

        Debug.LogError("Particle " + particle + " not found!");
        return null;
    }
}
