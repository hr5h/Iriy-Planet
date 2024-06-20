using JetBrains.Annotations;
using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ProgressBar : MonoBehaviour
{
    public Slider slider;
    public Gradient gradient;
    public Image fill;
    public TextMeshProUGUI healthPoint;

    public Human player;
    private void Start()
    {
        fill.color = gradient.Evaluate(1f);
    }

    public void UpdateBar()
    {
        if (Mathf.Ceil(player.Health) != Mathf.Ceil(slider.value))
        {
            healthPoint.text = Mathf.Ceil(player.Health).ToString();
            slider.maxValue = player.MaxHealth;
            slider.value = player.Health;
            fill.color = gradient.Evaluate(1f);
        }
    }

    public void SetHealth(float health)
    {
        healthPoint.text = Mathf.Ceil(health).ToString();
        slider.value = Mathf.Ceil(health);
        fill.color = gradient.Evaluate(slider.normalizedValue);
    }
}
