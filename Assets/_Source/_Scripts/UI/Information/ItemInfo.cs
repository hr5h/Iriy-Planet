using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ItemInfo : MonoBehaviour
{
    public GameObject btActivation;
    public GameObject btArmorEquip;
    public GameObject btArmorUnEquip;
    public GameObject btWeaponEquip;
    public GameObject btWeaponUnEquip;
    public GameObject btUse;
    public GameObject btDrop;
    public GameObject btUnload;
    public TextMeshProUGUI title;
    public ItemState itemState;

    private Canvas _canvas;

    private void Awake()
    {
        _canvas = GetComponent<Canvas>();
    }

    public void Show(InventoryCell inventoryCell)
    {
        _canvas.enabled = true;
        if (itemState != inventoryCell.state)
        {
            title.text = inventoryCell.state.data.Title;
            description.text = inventoryCell.state.data.Description;
            characteristics.text = " "; //TODO ItemInfo Внести основную информацию предмете
        }
    }

    public TextMeshProUGUI description;
    public TextMeshProUGUI characteristics;
    public Image icon;
}
