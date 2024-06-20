using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ReloadButton : ScreenButton
{
    private Image image;
    private PlayerControl playerControl;
    private Weapon weapon;
    private Color defaultColor;
    [SerializeField] private Color activeColor;
    void Start()
    {
        image = GetComponent<Image>();
        defaultColor = image.color;
        playerControl = WorldController.Instance.playerControl;
        playerControl.human.OnWeaponEquip.AddListener(WeaponEquipHandler);
        playerControl.human.OnWeaponUnEquip.AddListener(WeaponUnEquipHandler);
        playerControl.human.OnTakeAmmo.AddListener(TakeAmmoHandler);
        if (playerControl.human.weapon == null)
        {
            image.enabled = false;
        }
        else
        {
            weapon = playerControl.human.weapon;
            if (weapon.ammo < weapon.data.clip && playerControl.human.Ammo[playerControl.human.weapon.data.ammoType] > 0)
            {
                image.enabled = true;
            }
            WeaponEventsSubscribe(weapon);
        }
        ButtonDown.AddListener(playerControl.ReloadButton);
    }
    void WeaponShotHandler()
    {
        if (playerControl.human.Ammo[playerControl.human.weapon.data.ammoType] > 0)
        {
            image.enabled = true;
        }
        else
        {
            image.enabled = false;
        }
    }
    void TakeAmmoHandler(string type)
    {
        if (playerControl.human.weapon != null && type == playerControl.human.weapon.data.ammoType && playerControl.human.weapon.ammo < playerControl.human.weapon.data.clip)
        {
            WeaponShotHandler();
        }
    }
    void WeaponReloadBeginHandler()
    {
        image.color = activeColor;
    }
    void WeaponReloadCancelHandler()
    {
        image.color = defaultColor;
    }
    void WeaponReloadEndHandler()
    {
        image.color = defaultColor;
        if (weapon.ammo == weapon.data.clip || playerControl.human.Ammo[weapon.data.ammoType] == 0)
        {
            image.enabled = false;
        }
    }
    void WeaponEquipHandler(Weapon equipedWeapon)
    {
        if (weapon != null)
        {
            WeaponEventsUnscribe(weapon);
        }
        weapon = equipedWeapon;
        WeaponEventsSubscribe(weapon);
        if (weapon.ammo < weapon.data.clip && playerControl.human.Ammo[weapon.data.ammoType] > 0)
        {
            image.enabled = true;
        }
        else
        {
            image.enabled = false;
        }
    }
    void WeaponUnEquipHandler()
    {
        image.enabled = false;
        WeaponEventsUnscribe(weapon);
        weapon = null;
    }
    void WeaponEventsSubscribe(Weapon w)
    {
        w.OnShot.AddListener(WeaponShotHandler);
        w.OnReloadBegin.AddListener(WeaponReloadBeginHandler);
        w.OnReloadCancel.AddListener(WeaponReloadCancelHandler);
        w.OnReloadEnd.AddListener(WeaponReloadEndHandler);
    }
    void WeaponEventsUnscribe(Weapon w)
    {
        w.OnShot.RemoveListener(WeaponShotHandler);
        w.OnReloadBegin.RemoveListener(WeaponReloadBeginHandler);
        w.OnReloadCancel.RemoveListener(WeaponReloadCancelHandler);
        w.OnReloadEnd.RemoveListener(WeaponReloadEndHandler);
    }
}
