using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpeedProgression : MonoBehaviour
{
    int _currentColor;

    PlayerGravity _gravity;
    PropGenerator _propGenerator;

    [SerializeField] float spawnTimeDecay = .25f;
    [SerializeField] float speedIncrease = 5f;
    [Header("SkyBox")]
    [SerializeField] Color[] colors;

    void Awake()
    {
        _gravity = FindObjectOfType<PlayerGravity>();
        _propGenerator = FindObjectOfType<PropGenerator>();
    }

    void Start()
    {
        RenderSettings.skybox.SetColor("_SkyTint", colors[_currentColor]);
    }

    public void IncreaseSpeed()
    {
        _currentColor++;
        _currentColor = Mathf.Clamp(_currentColor, 0, colors.Length);

        _gravity.ForwardSpeed += speedIncrease;
        _propGenerator.TimeToSpawn -= spawnTimeDecay;

        RenderSettings.skybox.SetColor("_SkyTint", colors[_currentColor]);
    }
}
