using UnityEngine;

public class SpeedProgression : MonoBehaviour
{
    int _currentColor;
    Color _currentSkyboxColor;
    Color _currentFogColor;

    PlayerGravity _gravity;
    BerserkerMode _berserkerMode;
    PropGenerator _propGenerator;

    [SerializeField] float spawnTimeDecay = .25f;
    [SerializeField] float speedIncrease = 5f;
    [Header("SkyBox")]
    [SerializeField] Color[] skyboxColors;
    [SerializeField] Color[] fogColors;

    void Awake()
    {
        _gravity = FindObjectOfType<PlayerGravity>();
        _berserkerMode = FindObjectOfType<BerserkerMode>();
        _propGenerator = FindObjectOfType<PropGenerator>();
    }

    void Start()
    {
        SetSkyboxColor();
    }

    public void IncreaseSpeed()
    {
        _currentColor++;
        _currentColor = Mathf.Clamp(_currentColor, 0, skyboxColors.Length);

        _gravity.ForwardSpeed += speedIncrease;
        _propGenerator.TimeToSpawn -= spawnTimeDecay;

        if (!_berserkerMode.IsTransformed)
        {
            SetSkyboxColor();
        }
    }

    public void SetSkyboxColor()
    {
        _currentSkyboxColor = skyboxColors[_currentColor];
        _currentFogColor = fogColors[_currentColor];
        RenderSettings.skybox.SetColor("_SkyTint", _currentSkyboxColor);
        RenderSettings.fogColor = _currentFogColor;
    }
}
