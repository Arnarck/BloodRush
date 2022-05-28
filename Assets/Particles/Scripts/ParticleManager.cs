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

    public static void Play(ParticleType particle, float simulationSpeed)
    {
        ParticleSystem vfx = GetParticleEffect(particle);
        if (vfx != null)
        {
            vfx.gameObject.SetActive(true);
            vfx.Play();
            var mainModule = vfx.main;
            mainModule.simulationSpeed = simulationSpeed;

            if (vfx.transform.childCount < 0) return;

            for (int i = 0; i < vfx.transform.childCount; i++)
            {
                var main = vfx.transform.GetChild(i).GetComponent<ParticleSystem>().main;
                main.simulationSpeed = simulationSpeed / 5f;
            }
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
