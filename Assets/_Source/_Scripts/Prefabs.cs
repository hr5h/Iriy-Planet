using UnityEngine;
using AddressableTools;
using UnityEngine.AddressableAssets;

public static class Prefabs
{
    private const string itemsFolder = "Assets/_Source/Prefabs/Items/";

    public static Weapon weapon;
    public static Armor armor;
    public static Knife knife;
    public static Medicine medicine;
    public static Mineral mineral;
    public static Ammo ammo;
    public static ItemDefault itemDefault;
    public static ItemCollectable itemCollectable;
    public static DropItems dropItems;
    private static T LoadItemPrefab<T>() where T : MonoBehaviour
    {
        //Logger.Debug(itemsFolder + typeof(T).Name + ".prefab");
        return AddressableLoader.LoadAsset<GameObject>(itemsFolder + typeof(T).Name + ".prefab").GetComponent<T>();
    }
    private static void RemoveItemPrefab(MonoBehaviour prefab)
    {
        Addressables.Release(prefab.gameObject);
    }
    public static void Init()
    {
        weapon = LoadItemPrefab<Weapon>();
        armor = LoadItemPrefab<Armor>();
        knife = LoadItemPrefab<Knife>();
        medicine = LoadItemPrefab<Medicine>();
        mineral = LoadItemPrefab<Mineral>();
        ammo = LoadItemPrefab<Ammo>();
        itemCollectable = LoadItemPrefab<ItemCollectable>();
        itemDefault = LoadItemPrefab<ItemDefault>();
        dropItems = LoadItemPrefab<DropItems>();
    }
    public static void Release()
    {
        RemoveItemPrefab(weapon);
        RemoveItemPrefab(armor);
        RemoveItemPrefab(knife);
        RemoveItemPrefab(medicine);
        RemoveItemPrefab(mineral);
        RemoveItemPrefab(ammo);
        RemoveItemPrefab(itemCollectable);
        RemoveItemPrefab(itemDefault);
        RemoveItemPrefab(dropItems);
    }
}
