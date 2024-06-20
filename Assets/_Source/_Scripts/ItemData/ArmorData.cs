using UnityEngine;

[CreateAssetMenu(fileName = "New Armor", menuName = "Инвентарь/Броня")]
public class ArmorData : BaseItemData
{
    [Header("Бонус к броне")]
    public float armorBonus;
    [Header("Бонус к передвижению")]
    public float movementBonus;
    [Header("Максимальный износ")]
    public float conditionMax;
}