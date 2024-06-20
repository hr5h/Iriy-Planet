using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TargetInfo : MonoBehaviour
{
    public static TargetInfo Instance { get; private set; }
    [SerializeField] private TextMeshProUGUI _targetName;
    [SerializeField] private HealthBar _targetHealth;
    private Damageable _target;
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            _targetName.text = string.Empty;
        }
        else
        {
            Destroy(gameObject);
        }
    }
    public void SetTarget(Damageable damageable)
    {
        if (_target != null)
        {
            ResetTarget();
        }
        _target = damageable;
        _target.OnDeath.AddListener(ResetTarget);

        _targetName.text = _target.MyName;
        _targetHealth.SetOwner(_target);
    }
    public void ResetTarget(Damageable owner = null, Damageable killer = null)
    {
        _target.OnDeath.RemoveListener(ResetTarget);
        _target = null;

        _targetName.text = "";
        _targetHealth.ResetOwner(_target);
    }
}
