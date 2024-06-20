using UnityEngine;
public abstract class BaseItemData : ScriptableObject
{
    [Header("Иконка")]
    public Sprite Icon;
    [Header("Наименование")]
    public string Title;
    [Header("Описание")]
    [TextArea(4, 10)]
    public string Description;
    [Header("Стоимость")]
    public int Cost;
    [Header("Является ли квестовым")]
    public bool questItem;
    [Header("Ограничение на подбор предмета NPC")]
    public bool dontPickup;
}