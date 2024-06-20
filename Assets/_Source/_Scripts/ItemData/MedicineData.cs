using UnityEngine;

[CreateAssetMenu(fileName = "New Medicine", menuName = "Инвентарь/Медикамент")]
public class MedicineData : CollectableItemData
{
    [Header("Восстановление здоровья")]
    public int heal;
}