using UnityEngine;
using UnityEngine.UI;

public class HealthBarUI : MonoBehaviour
{
    public Slider healthSlider;

    public void SetMaxHealth(float max)
    {
        healthSlider.maxValue = max;
        healthSlider.value = max;
    }

    public void SetHealth(float current)
    {
        healthSlider.value = current;
    }
}