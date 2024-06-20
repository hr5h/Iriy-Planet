using Game.Sounds;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Кнопка на экране, отвечающая за быстрое лечение игрока
/// </summary>
public class MedicineButton : ScreenButton
{
    private Image _image;
    private PlayerControl _playerControl;
    private Color _defaultColor;
    [SerializeField] private Color _activeColor;

    bool _hasMedicine;
    bool _healingInProgress;
    bool _hasFullHealth;

    private void Start()
    {
        _image = GetComponent<Image>();
        _defaultColor = _image.color;
        _playerControl = WorldController.Instance.playerControl;

        _playerControl.OnFullHealth.AddListener(OnFullHealth);
        _playerControl.OnNotFullHealth.AddListener(OnNotFullHealth);
        _playerControl.human.inventory.OnInventoryUpdate.AddListener(OnInventoryUpdate);

        ButtonDown.AddListener(_playerControl.FastMedicine);
        
        if (_playerControl.human.Health == _playerControl.human.MaxHealth)
        {
            _hasFullHealth = true;
            _image.enabled = false;
        }
        else
        {
            _hasFullHealth = false;
            _image.enabled = true;
        }
    }
    private void OnInventoryUpdate(Inventory inv) // TODO не работает после торговли
    {
        _hasMedicine = Command.HasItem(inv, "bandage", 1) || Command.HasItem(inv, "medkitsmall", 1) || Command.HasItem(inv, "medkitmedium", 1);
        if (_hasMedicine && !_hasFullHealth)
        {
            _image.enabled = true;
        }
    }
    private void OnFullHealth()
    {
        _hasFullHealth = true;
        _image.enabled = false;
    }
    private void OnNotFullHealth()
    {
        _hasFullHealth = false;
        _image.enabled = _hasMedicine;
    }
    private void OnHealingStart()
    {
        _image.color = _activeColor;
        _healingInProgress = true;
    }
    private void OnHealingEnd()
    {
        _image.color = _defaultColor;
        _healingInProgress = false;
    }
}
