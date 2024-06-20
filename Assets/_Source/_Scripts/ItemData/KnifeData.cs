using UnityEngine;

[CreateAssetMenu(fileName = "New Knife", menuName = "Инвентарь/Холодное оружие")]
public class KnifeData : BaseItemData
{
    [Header("Урон")]
    public float damage;
    [Header("Интервал между ударами")]
    public float rate;
}