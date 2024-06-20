using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using System;

public class WeaponRecoilEffect : MonoBehaviour, IUpdatable
{
    [SerializeField] private float globalShakeForce = 1f;
    private PlayerControl _playerControl;
    private CinemachineImpulseSource _impulseSource;
    private Weapon _weapon;
    private void Awake()
    {
        _playerControl = GetComponent<PlayerControl>();
        _impulseSource = gameObject.GetComponent<CinemachineImpulseSource>();

        _playerControl.human.OnWeaponEquip.AddListener(OnWeaponEquip);
        _playerControl.human.OnWeaponUnEquip.AddListener(OnWeaponUnequip);

        if (_playerControl.human.weapon != null)
        {
            OnWeaponEquip(_playerControl.human.weapon);
        }
    }

    private void OnWeaponUnequip()
    {
        _weapon.OnShot.RemoveListener(OnWeaponShot);
        _weapon = null;
    }
    private void OnWeaponEquip(Weapon weapon)
    {
        if (_weapon != null)
        {
            OnWeaponUnequip();
        }
        _weapon = weapon;
        _weapon.OnShot.AddListener(OnWeaponShot);
    }
    private void OnWeaponShot()
    {
        ShakeCamera(_impulseSource);
    }

    private void OnEnable() => StartCoroutine(OnEnableCoroutine());
    private IEnumerator OnEnableCoroutine()
    {
        yield return null;
        WorldController.Instance.Updater.Add(this);
    }
    private void OnDisable()
    {
        WorldController.Instance.Updater.Remove(this);
    }
    public void ManualUpdate()
    {
        if ( Input.GetKeyDown(KeyCode.Space))
        {
            ShakeCamera(_impulseSource);
        }
    }

    public void ShakeCamera(CinemachineImpulseSource impulseSource)
    {
        impulseSource.GenerateImpulseWithForce(globalShakeForce);
    }
}
