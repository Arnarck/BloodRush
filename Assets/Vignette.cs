using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Vignette : MonoBehaviour
{
    bool _isPulseFinished;

    [SerializeField] Image vignette;
    [SerializeField] float minValue = .2f;
    [SerializeField] float intensity = 5f;
    [SerializeField] float transformedIntensity = 10f;
    [SerializeField] float maxValue = .7f;
    [SerializeField] float transformedValue = 1f;

    public void Activate()
    {
        StopAllCoroutines();
        StartCoroutine(ManagePulse());
    }

    public void Deactivate()
    {
        StopAllCoroutines();
        StartCoroutine(ApplyPulse(0f, intensity));
    }

    IEnumerator ManagePulse()
    {
        StartCoroutine(ApplyPulse(transformedValue, transformedIntensity));
        while (!_isPulseFinished)
        {
            yield return new WaitForEndOfFrame();
        }

        while (true)
        {
            StartCoroutine(ApplyPulse(minValue, intensity));
            while (!_isPulseFinished)
            {
                yield return new WaitForEndOfFrame();
            }

            StartCoroutine(ApplyPulse(maxValue, intensity));
            while (!_isPulseFinished)
            {
                yield return new WaitForEndOfFrame();
            }
        }
    }

    IEnumerator ApplyPulse(float finalValue, float speed)
    {
        float pulsePercentage = 0f;
        float initialValue = vignette.color.a;

        _isPulseFinished = false;
        while (pulsePercentage < 1f)
        {
            float valueChangeThisFrame = Time.deltaTime * speed;
            pulsePercentage += valueChangeThisFrame;
            vignette.color = new Color(vignette.color.r, vignette.color.g, vignette.color.b, Mathf.Lerp(initialValue, finalValue, pulsePercentage));

            yield return new WaitForEndOfFrame();
        }
        _isPulseFinished = true;
    }
}
