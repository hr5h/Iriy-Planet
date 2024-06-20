using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

//СКРИПТ ДЛЯ ВЫПОЛНЕНИЯ СПЕЦИАЛЬНЫХ КОМАНД
public class Command : MonoBehaviour
{
    //TODO избавиться от статических полей и методов
    public static Command Instance { get; private set; }
    public static Dictionary<string, BaseItemData> items = new Dictionary<string, BaseItemData>();
    public static HashSet<string> tasks = new HashSet<string>(); //Все выданные когда-либо задачи

    public static string[] weaponNames = { "ak74", "energygun", "lasercanon", "lasergun", "pistol", "plasmagun", "uzi" };
    public static string[] ammotypes = { "5.45", "energy", "laser_large", "laser", "9mm", "plasma", "9mm" };

    public static Dictionary<string, BaseItemData> weapons = new Dictionary<string, BaseItemData>();
    public static Dictionary<string, BaseItemData> armors = new Dictionary<string, BaseItemData>();
    public static Dictionary<string, BaseItemData> minerals = new Dictionary<string, BaseItemData>();
    public static Dictionary<string, BaseItemData> ammo = new Dictionary<string, BaseItemData>();
    public static Dictionary<string, BaseItemData> others = new Dictionary<string, BaseItemData>();
    public static Dictionary<string, BaseItemData> medicals = new Dictionary<string, BaseItemData>();
    public static Dictionary<string, BaseItemData> knifes = new Dictionary<string, BaseItemData>();

    private static List<AsyncOperationHandle<IList<ScriptableObject>>> addressableHandlers = new List<AsyncOperationHandle<IList<ScriptableObject>>>();

    //Закрывает активные окна (Инвентарь, журнал, задачи и т.д.)
    public static void CloseAllWindow()
    {
        if (InventoryController.Instance.inventory)
            InventoryController.Instance.InventoryClose();
        if (InventoryController.Instance.trading)
            InventoryController.Instance.TradingClose();
        if (InventoryController.Instance.exchange)
            InventoryController.Instance.ExchangeClose();
        if (HistoryController.instance.open)
            HistoryController.instance.HistoryClose();
        if (TaskController.instance.open)
            TaskController.instance.JournalClose();
    }
    //Проверка, закрыты ли все окна
    public static bool AllWindowsClosed()
    {
        return ((!InventoryController.Instance.inventory) &&
            (!InventoryController.Instance.trading) &&
            (!InventoryController.Instance.exchange) &&
            (!DialogController.instance.open) &&
            (!HistoryController.instance.open) &&
            (!TaskController.instance.open));
    }
    //Удалить предмет из инвентаря (только одного)
    public static void RemoveItem(Inventory inventory, string data, int count = 1)
    {
        data = data.ToLower();
        for (int i = 0; i < inventory.items.Count; i++)
        {
            if (inventory.items[i] != null && inventory.items[i].data.GetType() == items[data].GetType())
            {
                //TODO RemoveItem добавить возможность удалять count предметов (щас 1 максимум)
                if (inventory.items[i] is CollectableItemState)
                {
                    (inventory.items[i] as CollectableItemState).count--;
                    if ((inventory.items[i] as CollectableItemState).count == 0)
                    {
                        inventory.Remove(i);
                    }
                }
                else
                {
                    inventory.Remove(i);
                }
                return;
            }
        }
    }
    //Создать предмет и передать его игроку
    public static void GiveItem(Inventory inventory, string data, int count = 1)
    {
        data = data.ToLower();
        if (!items.ContainsKey(data))
        {
            Logger.Debug($"Предмета {data} не существует!");
            return;
        }

        var pos = inventory.place.transform.position;
        var rot = Quaternion.Euler(0, 0, Random.Range(0f, 360f));

        switch (items[data])
        {
            case WeaponData item:
                {
                    for (int i = 0; i < count; i++)
                    {
                        var state = new WeaponState
                        {
                            data = item,
                            condition = item.conditionMax,
                            ammo = item.clip
                        };
                        if (!inventory.Add(state))
                        {
                            var weapon = Instantiate(Prefabs.weapon, pos, rot);
                            weapon.Init(state);
                        }
                    }
                    break;
                }
            case MineralData item:
                {
                    for (int i = 0; i < count; i++)
                    {
                        var state = new MineralState
                        {
                            data = item
                        };
                        if (!inventory.Add(state))
                        {
                            var mineral = Instantiate(Prefabs.mineral, pos, rot);
                            mineral.data = item;
                        }
                    }
                    break;
                }
            case ArmorData item:
                {
                    for (int i = 0; i < count; i++)
                    {
                        var state = new ArmorState
                        {
                            data = item,
                            condition = item.conditionMax
                        };
                        if (!inventory.Add(state))
                        {
                            var armor = Instantiate(Prefabs.armor, pos, rot);
                            armor.data = item;
                            armor.condition = state.condition;
                        }
                    }
                    break;
                }
            case KnifeData item:
                {
                    for (int i = 0; i < count; i++)
                    {
                        var state = new KnifeState
                        {
                            data = item
                        };
                        if (!inventory.Add(state))
                        {
                            var knife = Instantiate(Prefabs.knife, pos, rot);
                            //knife.data = item;
                        }
                    }
                    break;
                }
            case MedicineData item:
                {
                    for (int i = 0; i < count; i++)
                    {
                        var state = new MedicineState
                        {
                            data = item,
                            count = 1
                        };
                        if (!inventory.Add(state))
                        {
                            var medicine = Instantiate(Prefabs.medicine, pos, rot);
                            medicine.data = item;
                        }
                    }
                    break;
                }
            case AmmoData item:
                {
                    var state = new AmmoState
                    {
                        data = item,
                        count = count
                    };
                    if (inventory.owner != null)
                    {
                        inventory.owner.TakeAmmo(item.ammoType, count);
                    }
                    if (!inventory.Add(state))
                    {
                        var ammo = Instantiate(Prefabs.ammo, pos, rot);
                        ammo.data = item;
                        ammo.count = state.count;
                        if (inventory.owner != null)
                        {
                            inventory.owner.TakeAmmo(item.ammoType, -count);
                        }
                    }
                    break;
                }
            case CollectableItemData item:
                {
                    for (int i = 0; i < count; i++)
                    {
                        var state = new CollectableItemState
                        {
                            data = item
                        };
                        if (!inventory.Add(state))
                        {
                            var item_collectable = Instantiate(Prefabs.itemCollectable, pos, rot);
                            item_collectable.data = item;
                        }
                    }
                    break;
                }
            default:
                {
                    for (int i = 0; i < count; i++)
                    {
                        var item = items[data];
                        var state = new ItemState
                        {
                            data = item
                        };
                        if (!inventory.Add(state))
                        {
                            var item_default = Instantiate(Prefabs.itemDefault, pos, rot);
                            item_default.data = (ItemData)item;
                        }
                    }
                    break;
                }
        }
    }
    //Создать предмет на карте
    public static GameObject[] CreateItem(string data, Vector2 pos, Quaternion rot, int count = 1)
    {
        data = data.ToLower();
        if (!items.ContainsKey(data))
            return null;
        switch (items[data])
        {
            case WeaponData item:
                {
                    var res = new GameObject[count];
                    for (int i = 0; i < count; i++)
                    {
                        var state = new WeaponState
                        {
                            data = item,
                            condition = item.conditionMax,
                            ammo = item.clip
                        };

                        var weapon = Instantiate(Prefabs.weapon, pos, rot);
                        weapon.Init(state);
                        res[i] = weapon.gameObject;
                    }
                    return res;
                }
            case MineralData _:
                {
                    var res = new GameObject[count];
                    for (int i = 0; i < count; i++)
                    {
                        var mineral = Instantiate(Prefabs.mineral, pos, rot);
                        mineral.data = (MineralData)items[data];
                        res[i] = mineral.gameObject;
                    }
                    return res;
                }
            case ArmorData item:
                {
                    var res = new GameObject[count];
                    for (int i = 0; i < count; i++)
                    {
                        var armor = Instantiate(Prefabs.armor, pos, rot);
                        armor.data = item;
                        armor.condition = item.conditionMax;
                        res[i] = armor.gameObject;
                    }
                    return res;
                }
            case KnifeData item:
                {
                    var res = new GameObject[count];
                    for (int i = 0; i < count; i++)
                    {
                        var knife = Instantiate(Prefabs.knife, pos, rot);
                        res[i] = knife.gameObject;
                        //knife.data = item;
                    }
                    return res;
                }
            case MedicineData _:
                {
                    var res = new GameObject[count];
                    for (int i = 0; i < count; i++)
                    {
                        var medicine = Instantiate(Prefabs.medicine, pos, rot);
                        medicine.data = (MedicineData)items[data];
                        res[i] = medicine.gameObject;
                    }
                    return res;
                }
            case AmmoData item:
                {
                    var res = new GameObject[1];

                    var ammo = Instantiate(Prefabs.ammo, pos, rot);
                    ammo.data = item;
                    ammo.count = count;
                    res[0] = ammo.gameObject;
                    return res;
                }
            case CollectableItemData item:
                {
                    var res = new GameObject[count];
                    for (int i = 0; i < count; i++)
                    {
                        var item_collectable = Instantiate(Prefabs.itemCollectable, pos, rot);
                        item_collectable.data = item;
                        res[i] = item_collectable.gameObject;
                    }
                    return res;
                }
            default:
                {
                    var res = new GameObject[count];
                    for (int i = 0; i < count; i++)
                    {
                        var item_default = Instantiate(Prefabs.itemDefault, pos, rot);
                        item_default.data = (ItemData)items[data];
                        res[i] = item_default.gameObject;
                    }
                    return res;
                }
        }
    }

    //Дать деньги персонажу
    public static void GiveMoney(Human human, int count)
    {
        human.TakeMoney(count);
    }
    //Проверить есть ли предмет в инвентаре в заданном количестве
    public static bool HasItem(Inventory inv, string data, int count = 1)
    {
        for (int i = 0; i < inv.items.Count; i++)
        {
            if ((inv.items[i] != null) && (inv.items[i].data.Title == items[data].Title)) //TODO сравнение по строкам неэффективно. Каждому предмету нужен идентификатор
            {
                if (inv.items[i] is CollectableItemState)
                {
                    count -= (inv.items[i] as CollectableItemState).count;
                }
                else
                {
                    count--;
                }
                if (count <= 0) return true;
            }
        }
        return false;
    }
    //public static bool HasAnyItemOf(Inventory inv, string[] data)
    //{
    //TODO HasAnyItemOf
    //}
    //Проверить есть деньги в заданном количестве у персонажа
    public static bool HasMoney(Human human, int money)
    {
        return (human.money <= money);
    }
    //Проверить, выдавалось ли игроку задание
    public static bool HasTask(string title)
    {
        return tasks.Contains(title);
    }
    public static bool HasActiveTask(string title)
    {
        foreach (var x in TaskController.instance.tasks)
        {
            if (x.title == title) return true;
        }
        return false;
    }
    //Создать НПС
    //public static Human SpawnNPC(Vector2 pos, string clan)
    //{
    //    var npc = Instantiate(Prefabs.human, pos, Quaternion.Euler(0, 0, 0));
    //    var ai = Instantiate(Prefabs.humanAIDefault, pos, Quaternion.Euler(0, 0, 0));
    //    ai.owner = npc;
    //    npc.ai = ai;
    //    npc.DefaultValues(clan);
    //    return npc;
    //}

    //Создание текстового сообщения
    public static Message ShowMessage(string title, string text, Color col, float time = 10, bool inHistory = true)
    {
        Logger.Debug("Сообщения отключены!");
        return null;
        //return MessageController.instance.ShowMessage(title, text, col, time, inHistory);
    }

    //Выдать задание
    public static void AddTask(string title, string description)
    {
        tasks.Add(title);
        ShowMessage("Задание получено:", "\"" + title + "\"", Color.white);
        TaskController.instance.AddTask(title, description);
    }
    //Выполнить задание
    public static void CompleteTask(string title)
    {
        ShowMessage("Задание выполнено:", "\"" + title + "\"", Color.green);
        TaskController.instance.RemoveTask(title);
    }
    //Провалить задание
    public static void FailTask(string title)
    {
        ShowMessage("Задание провалено:", "\"" + title + "\"", Color.red);
        TaskController.instance.RemoveTask(title);
    }
    //Отменить задание
    public static void CancelTask(string title)
    {
        ShowMessage("Задание отменено:", "\"" + title + "\"", Color.white);
        TaskController.instance.RemoveTask(title);
    }
    //Обновить задание
    public static void UpdateTask(string title, string description)
    {
        ShowMessage("Задание обновлено:", "\"" + title + "\"", Color.white);
        TaskController.instance.UpdateTask(title, description);
    }
    void Awake()
    {
        Instance = this;
        tasks.Clear();

        LoadAllItems();
    }
    private void OnDestroy()
    {
        ReleaseAllItems();
    }
    
    //TODO перенести логику загрузки ассетов в AddressableLoader
    void LoadItems( string label, Dictionary<string, BaseItemData> dict)
    {
        List<ScriptableObject> result = new List<ScriptableObject>();
        addressableHandlers.Add(AddressableTools.AddressableLoader.LoadAsset(label, result));
        foreach(var x in result) 
        {
            if (!dict.ContainsKey(x.name.ToLower()))
                dict.Add(x.name.ToLower(), x as BaseItemData);
        }
    }

    void LoadAllItems()
    {
        LoadItems("ItemWeapon", weapons);
        LoadItems("ItemMineral", minerals);
        LoadItems("ItemArmor", armors);
        LoadItems("ItemMedicine", medicals);
        LoadItems("ItemOther", others);
        LoadItems("ItemKnife", knifes);
        LoadItems("ItemAmmo", ammo);

        items = weapons
            .Concat(armors)
            .Concat(minerals)
            .Concat(ammo)
            .Concat(others)
            .Concat(medicals)
            .Concat(knifes)
            .ToDictionary(pair => pair.Key, pair => pair.Value);
    }
    void ReleaseAllItems()
    {
        foreach( var handler in addressableHandlers)
        {
            Addressables.Release(handler);
        }
        addressableHandlers.Clear();
    }

    public static string[] names = { "Егор", "Аркаша", "Сёма", "Валера", "Женя", "Костян", "Виталя", "Вася",
        "Петя", "Вова", "Илья", "Денис", "Антон", "Коля", "Семён", "Сергей", "Макс", "Олег", "Саша", "Федя",
        "Слава", "Никита", "Андрей", "Трофим", "Миша", "Ваня", "Гоша", "Стас", "Стёпа", "Влад", "Вадим", "Кирилл",
        "Глеб", "Лёха", "Ефим"};

    public static string[] nicknames = { "Клоун", "Жёлудь", "Шкаф", "Кабан", "Вист", "Верзила", "Хвост", "Шут",
        "Крот", "Кантер", "Умка", "Крюк", "Бык", "Скряга", "Колобок", "Винтик", "Лохматый", "Барон", "Куница",
        "Гора", "Хомяк", "Муха", "Рыжий", "Ёжик", "Грач", "Бурак", "Заяц", "Хек", "Болтун", "Руль", "Сова",
        "Зуб", "Полторашка", "Печка", "Чиж", "Непонятный"};
    public static string RandomName()
    {
        return names[Random.Range(0, names.Length)] + " " + nicknames[Random.Range(0, nicknames.Length)];
    }

    public static void RandomWeapon(Human human) //Дать персонажу случайное оружие и патроны к нему
    {
        int indWeapon = Random.Range(0, weaponNames.Length);
        int countAmmo = (items[ammotypes[indWeapon]] as AmmoData).countMax * Random.Range(1, 4);
        CreateItem(weaponNames.ElementAt(indWeapon), human.transform.position, Quaternion.Euler(0f, 0f, Random.Range(0, 360)))[0].GetComponent<Weapon>().Equip(human);
        GiveItem(human.inventory, ammotypes[indWeapon], countAmmo);
    }
    public static void RandomArmor(Human human) //Надеть случайную броню на персонажа
    {
        //TODO RandomArmor(human);
    }
    public static void RandomInventory(Human human) // Наполнить инвентарь случайными предметами
    {
        //TODO RandomInventory();
    }

}
