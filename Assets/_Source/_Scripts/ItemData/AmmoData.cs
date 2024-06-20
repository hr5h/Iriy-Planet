using UnityEngine;

[CreateAssetMenu(fileName = "New Ammo", menuName = "Инвентарь/Патроны")]
public class AmmoData : CollectableItemData
{
    [Header("Отсечка при перемещении в ячейку")]
    public int ammoCount;
    [Header("Калибр")]
    public string ammoType;
}
