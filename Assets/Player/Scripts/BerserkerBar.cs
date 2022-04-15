using UnityEngine;
using UnityEngine.UI;

public class BerserkerBar : MonoBehaviour
{
    public float MinValue { get => bar.minValue;}
    public float CurrentValue { get => bar.value; set => bar.value = value; }
    public float MaxValue { get => bar.maxValue; set => bar.maxValue = value; }

    [SerializeField] Slider bar;
    [SerializeField] float maxValue;
    [SerializeField] float startValue;

    // Start is called before the first frame update
    void Start()
    {
        MaxValue = maxValue;
        CurrentValue = startValue;
    }

    // Increases / Decreases the current value
    public void ModifyCurrentValue(float amount)
    {
        CurrentValue += amount;
    }
}
