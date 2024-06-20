using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ArmorBar : MonoBehaviour
{
    public Slider slider;
    public Gradient gradient;
    public Image fill;
    public TextMeshProUGUI armorPoint;

    public Human player;

    //public void SetArmor()
    //{
    //    armorPoint.text = Mathf.Ceil(100).ToString();
    //    slider.maxValue = def;
    //    slider.value = def;
    //    fill.color = gradient.Evaluate(1f);
    //}
    public void Start()
    {
        slider.maxValue = 100;
        fill.color = gradient.Evaluate(1f);
        player.OnArmorEquip.AddListener(SetArmor);
        player.OnArmorDamaged.AddListener(SetArmor);
        player.OnMineralEquip.AddListener(SetArmor);
        player.OnMineralUnEquip.AddListener(SetArmor);
        player.OnArmorUnEquip.AddListener(SetArmor);
    }
    public void SetArmor()
    {
        armorPoint.text = Mathf.RoundToInt(player.defence).ToString();
        slider.value = Mathf.RoundToInt(player.defence);
        fill.color = gradient.Evaluate(slider.normalizedValue);
    }
}
