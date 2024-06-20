using UnityEngine;

[CreateAssetMenu(fileName = "New Weapon", menuName = "Инвентарь/Огнестрельное оружие")]
public class WeaponData : BaseItemData
{
    [Header("Тип патронов")]
    public string ammoType;
    [Header("Размер обоймы")]
    public int clip;
    [Header("Длительность перезарядки")]
    public float reloadTime;
    [Header("Максимальный износ")]
    public int conditionMax;
    [Header("Урон")]
    public float damage;
    [Header("Минимальный разброс пуль")]
    public float spreadMin;
    [Header("Максимальный разброс пуль")]
    public float spreadMax;
    [Header("Скорость снижения разброса")]
    public float spreadReduction;
    [Header("Возрастание разброса при выстреле")]
    public float spreadIncreace;
    [Header("Интервал между выстрелами")]
    public float rate;
    [Header("Дальность стрельбы")]
    public float range;
    [Header("Скорость пули")]
    public float bulletSpeed;
    [Header("Снижение скорости передвижения")]
    public float movementCoef;
    [Header("Является ли одноручным")]
    public bool oneHanded;
    [Header("Цвет пули")]
    public Color bulletColor;
    [Header("Цвет выстрела")]
    public Color firePointColor;
    [Header("Размер пули")]
    public float bulletWidth;
    [Header("Время исчезновения следа от пули")]
    public float bulletTrailTime;
    [Header("Масса пули")]
    public float bulletMass;
    [Header("Стреляет дробью")]
    public bool IsShotgun;
    [Header("Теряется ли урон с увеличением дистанции")]
    public bool damageReduce;
    [Header("Звук при выстреле")]
    public AudioClip shootAudio;

    public string PrintHelp()
    {
        if (oneHanded)
            return "Является одноручным";
        else
            return "Не является одноручным";
    }
}
