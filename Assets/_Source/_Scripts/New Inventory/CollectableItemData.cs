using UnityEngine;
[CreateAssetMenu(fileName = "New CollectableItem", menuName = "Инвентарь/Количественный предмет")]
public class CollectableItemData : BaseItemData
{
    [Header("Максимальное количество в ячейке инвентаря")]
    public int countMax;
}
