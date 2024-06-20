using UnityEngine;

[CreateAssetMenu(fileName = "New Mineral", menuName = "Инвентарь/Минерал")]
public class MineralData : BaseItemData
{
    [Header("Бонус к передвижению")]
    public float movementBonus;
    [Header("Бонус к здоровью")]
    public float hpBonus;
    [Header("Бонус к регенерации")]
    public float regenBonus;
    [Header("Бонус к броне")]
    public float armorBonus;

    public string PrintHelp()
    {
        string res = "";
        if (movementBonus != 0.0)
        {
            res += "<b>Бонус к передвижению:</b> " + movementBonus.ToString() + "\n\n";
        }
        if (hpBonus != 0.0)
        {
            res += "<b>Бонус к здоровью:</b> " + hpBonus.ToString() + "\n\n";
        }
        if (regenBonus != 0.0)
        {
            res += "<b>Бонус к регенерации:</b> " + regenBonus.ToString() + "\n\n";
        }
        if (armorBonus != 0.0)
        {
            res += "<b>Бонус к броне:</b> " + armorBonus.ToString() + "\n\n";
        }
        return res;
    }
}
