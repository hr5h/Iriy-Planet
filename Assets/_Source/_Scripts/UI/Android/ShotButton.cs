using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ShotButton : ScreenButton, IPointerUpHandler
{
    [HideInInspector] public UnityEvent ButtonUp;
    private Color defaultColor;
    [SerializeField] private Color activeColor;
    private PlayerControl playerControl;
    private Image image;
    private Weapon weapon;
    private float timer = 0f;
    public void OnPointerUp(PointerEventData eventData)
    {
        ButtonUp?.Invoke();
    }

    void Start()
    {
        image = GetComponent<Image>();
        defaultColor = image.color;
        playerControl = WorldController.Instance.playerControl;
        if (playerControl.human.weapon != null)
        {
            weapon = playerControl.human.weapon;
            weapon.OnShot.AddListener(WeaponShotHandler);
            image.enabled = true;
        }
        else
            image.enabled = false;

        playerControl.human.OnWeaponEquip.AddListener(WeaponEquipHandler);
        playerControl.human.OnWeaponUnEquip.AddListener(WeaponUnEquipHandler);

        ButtonDown.AddListener(playerControl.ShotButtonDown);
        ButtonUp.AddListener(playerControl.ShotButtonUp);
    }
    void Update()
    {
        if (timer > 0f)
        {
            timer -= Time.deltaTime;
            if (timer <= 0f)
            {
                image.color = defaultColor;
            }
        }
    }
    void WeaponShotHandler()
    {
        image.color = activeColor;
        timer = weapon.data.rate + 0.05f;
    }
    void WeaponEquipHandler(Weapon w)
    {
        if (weapon != null)
        {
            weapon.OnShot.RemoveListener(WeaponShotHandler);
        }
        weapon = w;
        weapon.OnShot.AddListener(WeaponShotHandler);
        image.enabled = true;
    }
    void WeaponUnEquipHandler()
    {
        weapon.OnShot.RemoveListener(WeaponShotHandler);
        weapon = null;
        image.enabled = false;
    }
}
