using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{
    [SerializeField] private Damageable _owner;
    private Slider _slider;

    [SerializeField] private Image _background;
    [SerializeField] private Image _fill;

    private void Awake()
    {
        Hide();
        _slider = GetComponent<Slider>();
    }
    private void Start()
    {
        if (_owner != null)
        {
            Show();
            _owner.OnHealthChanged.AddListener(UpdateBar);
        }
    }
    public void SetOwner(Damageable damageable)
    {
        if (_owner != null)
        {
            ResetOwner();
        }
        _owner = damageable;
        _owner.OnHealthChanged.AddListener(UpdateBar);
        UpdateBar();
        Show();
    }
    public void ResetOwner(Damageable owner = null, Damageable killer = null)
    {
        _owner.OnHealthChanged.RemoveListener(UpdateBar);
        _owner = null;
        Hide();
    }
    private void UpdateBar()
    {
        if (_slider.maxValue != _owner.MaxHealth)
        {
            _slider.maxValue = _owner.MaxHealth;
        }
        if (Mathf.Ceil(_slider.value) != Mathf.Ceil(_owner.Health))
        {
            _slider.value = _owner.Health;
        }
    }

    public void Show()
    {
        _background.enabled = true;
        _fill.enabled = true;
    }
    public void Hide()
    {
        _background.enabled = false;
        _fill.enabled = false;
    }
}
