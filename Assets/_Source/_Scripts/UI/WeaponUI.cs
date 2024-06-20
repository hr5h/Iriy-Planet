using TMPro;
using UnityEngine;

public class WeaponUI : MonoBehaviour
{
    [HideInInspector] public TextMeshProUGUI textMesh;
    public TextMeshProUGUI textMesh2;
    [HideInInspector] public bool active;
    public Human player = null;

    private Color defaultColor;
    [SerializeField] Color warningColor;

    private int _oldClipAmmo = int.MinValue;
    private int _oldStorageAmmo = int.MinValue;
    public void Start()
    {
        textMesh = GetComponent<TextMeshProUGUI>();
        defaultColor = textMesh.color;
        active = false;
    }
    void UpdateText()
    {
        if ( player.weapon.ammo != _oldClipAmmo)
        {
            _oldClipAmmo = player.weapon.ammo;
            textMesh.text = player.weapon.ammo.ToString() + "/";

            //Если в обойме осталась только четверть патронов - предупредить игрока красным цветом
            if ((float)player.weapon.ammo / player.weapon.data.clip <= 0.25f)
            {
                textMesh.color = warningColor;
                textMesh2.color = warningColor;
            }
            else
            {
                textMesh.color = defaultColor;
                textMesh2.color = defaultColor;
            }
        }
        if (player.Ammo[player.weapon.data.ammoType] != _oldStorageAmmo)
        {
            _oldStorageAmmo = player.Ammo[player.weapon.data.ammoType];
            textMesh2.text = player.Ammo[player.weapon.data.ammoType].ToString();
        }

    }
    public void ClearText()
    {
        textMesh.text = "";
        textMesh2.text = "";
        _oldClipAmmo = int.MinValue;
        _oldStorageAmmo = int.MinValue;
    }
    void Update()
    {
        if (player != null)
        {
            if (active)
                UpdateText();
        }
        else Destroy(gameObject);
    }
}
