using Game.Sounds;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Inventory : MonoBehaviour
{
    public int width;
    public int height;
    public Human owner;
    public GameObject place; //Объект, который владеет инвентарем. В отличие от owner - это не обязательно человек
    public List<ItemState> items = new List<ItemState>();
    public delegate void ItemUpdate(int ind);
    public event ItemUpdate OnItemUpdate;
    [HideInInspector] public UnityEvent<int> OnCellUpdate; ///Событие обновления ячейки
    [HideInInspector] public UnityEvent<Inventory> OnInventoryUpdate; ///Событие обновления инвентаря
    [HideInInspector] public string deadName;

    //AudioSource
    public AudioSource audioSource;
    //AudioClip
    public AudioClip addItemClip;

    void Awake()
    {
        for (int i = 0; i < width * height; i++)
        {
            items.Add(null);
        }
        if (owner != null)
        {
            owner.OnReload.AddListener(RemoveAmmo);
        }
    }
    void ClearAll() //Удаляет все предметы из инвентаря
    {
        for (int i = 0; i < width * height; i++)
        {
            Remove(i);
        }
        OnInventoryUpdate?.Invoke(this);
    }
    void Swap(int a, int b) //Меняет местами два предмета
    {
        (items[a], items[b]) = (items[b], items[a]);
    }
    public bool IsEmpty() //Проверка, есть ли свободная ячейка
    {
        for (int i = 0; i < items.Count; i++)
        {
            if (items[i] == null) return true;
        }
        return false;
    }
    public bool IsEmpty(CollectableItemData data) //Проверка, есть ли место для количественных предметов
    {
        for (int i = 0; i < items.Count; i++)
        {
            if (items[i] == null)
            {
                return true;
            }
            else
            {
                if ((items[i].data.GetType() == data.GetType()) && ((items[i] as CollectableItemState).count < data.countMax))
                {
                    //Если нашли ячейку подходящего типа, которая ещё не заполнена полностью, то считать её свободной
                    return true;
                }
            }
        }
        return false;
    }
    public void Drop(int a) //Выбрасывает предмет из инвентаря
    {

    }
    public void Remove(int a) //Удаляет предмет из указанной ячейки
    {
        items[a] = null;
        OnInventoryUpdate?.Invoke(this);
        //OnItemUpdate?.Invoke(a);
    }
    public void RemoveAmmo(string type, int count) //Удаление патронов из инвентаря, после перезарядки
    {
        for (int i = 0; i < width * height; i++)
        {
            if (items[i] != null)
            {
                if (items[i] is AmmoState)
                {
                    var ammo = items[i] as AmmoState;
                    if ((ammo.data as AmmoData).ammoType == type)
                    {
                        (ammo.count, count) = (Mathf.Max(0, ammo.count - count), Mathf.Max(0, count - ammo.count));
                        if (ammo.count == 0)
                        {
                            Remove(i);
                        }
                        OnItemUpdate?.Invoke(i);
                        if (count == 0)
                        {
                            OnInventoryUpdate?.Invoke(this);
                            return;
                        }
                    }
                }
            }
        }
        OnInventoryUpdate?.Invoke(this);
    }
    public bool Add(ItemState item) //Добавляет предмет в свободный слот. Возвращает true, если удалось добавить предмет
    {
        if (owner && owner.mainHero)
        {
            AudioPlayer.Instance.PlaySoundFX(addItemClip, audioSource);
        }
        bool res = false;
        if (item == null)
        {
            return true;
        }
        if (item is CollectableItemState) //Если item - количественный предмет, то проверяем, есть ли уже ячейки с таким предметом
        {
            for (int i = 0; i < width * height; i++)
            {
                if ((items[i] != null) && (items[i].data == item.data)) //Нашли ячейку с таким же предметом
                {
                    //Заполняем её
                    var t1 = item as CollectableItemState;
                    var t2 = items[i] as CollectableItemState;
                    var countMax = (t1.data as CollectableItemData).countMax;
                    if ((t2.count != countMax) && (t2.count + t1.count <= countMax))
                    {
                        t2.count += t1.count;
                        t1.count = 0;
                    }
                    else
                    {
                        if (t2.count != countMax)
                            res = true;
                        (t1.count, t2.count) = (t1.count + t2.count - countMax, countMax);
                    }
                    OnItemUpdate?.Invoke(i);
                    if (t1.count == 0)
                    {
                        OnInventoryUpdate?.Invoke(this);
                        return true;
                    }
                }
            }
        }
        for (int i = 0; i < width * height; i++)
        {
            if (items[i] == null)
            {
                res = true;
                if ((item is CollectableItemState))
                {
                    var t1 = item as CollectableItemState;
                    var countMax = (t1.data as CollectableItemData).countMax;
                    if (t1.count <= (countMax))
                    {
                        items[i] = item;
                        OnItemUpdate?.Invoke(i);
                        OnInventoryUpdate?.Invoke(this);
                        return true;
                    }
                    else
                    {
                        switch (item)
                        {
                            case AmmoState _: { items[i] = new AmmoState(); break; }
                            case MedicineState _: { items[i] = new MedicineState(); break; }
                            default: { items[i] = new CollectableItemState(); break; }
                        }
                        items[i].data = item.data;
                        (items[i] as CollectableItemState).count = countMax;
                        t1.count -= countMax;
                        OnItemUpdate?.Invoke(i);
                    }
                }
                else
                {
                    items[i] = item;
                    OnItemUpdate?.Invoke(i);
                    OnInventoryUpdate?.Invoke(this);
                    return true;
                }
                //switch (item.GetType().Name)
                //{
                //    case nameof(WeaponState): { items[i] = item as WeaponState; break; }
                //    case nameof(MineralState): { items[i] = item as MineralState; break; }
                //    case nameof(ArmorState): { items[i] = item as ArmorState; break; }
                //    case nameof(KnifeState): { items[i] = item as KnifeState; break; }
                //    case nameof(ItemState): { items[i] = item; break; }
                //}
            }
            //else
            //{
            //    if ((item is CollectableItemState) && (items[i] is CollectableItemState))
            //    {
            //        if (item.data.name == items[i].data.name)
            //        {
            //            var t1 = item as CollectableItemState;
            //            var t2 = items[i] as CollectableItemState;
            //            var countMax = (t1.data as CollectableItemData).countMax;
            //            t2.count += t1.count;
            //            if (t2.count>countMax)
            //            {
            //                t1.count = t2.count - countMax;
            //                t2.count = countMax;
            //            }
            //            else
            //            {
            //                t1.count = 0;
            //            }
            //            OnItemUpdate?.Invoke(i);
            //            if (t1.count == 0)
            //            {
            //                return true;
            //            }
            //        }
            //    }
            //}
        }
        OnInventoryUpdate?.Invoke(this);
        return res;
    }
    public bool Add(CollectableItemState item, int count) //Добавление заданного числа количественных предметов в инвентарь
    {
        if ((count <= 0) || (item == null))
        {
            return true;
        }
        if (count > item.count)
            count = item.count;

        for (int i = 0; i < width * height; i++)
        {
            if ((items[i] != null) && (items[i].data == item.data)) //Нашли ячейку с таким же предметом
            {
                //Заполняем её
                var t2 = items[i] as CollectableItemState;
                var countMax = (item.data as CollectableItemData).countMax;
                if ((t2.count != countMax) && (t2.count + count <= countMax))
                {
                    t2.count += count;
                    item.count -= count;
                    count = 0;
                }
                else
                {
                    (count, item.count, t2.count) = (count + t2.count - countMax, item.count + t2.count - countMax, countMax);
                }
                OnItemUpdate(i);
                if (item.count == 0)
                {
                    OnInventoryUpdate?.Invoke(this);
                    return true;
                }
            }
        }
        CollectableItemState item2;
        switch (item)
        {
            case AmmoState _: { item2 = new AmmoState(); break; }
            case MedicineState _: { item2 = new MedicineState(); break; }
            default: { item2 = new CollectableItemState(); break; }
        }
        item2.count = count;
        item2.data = item.data;
        bool b = Add(item2);
        if (b)
            item.count -= item2.count;
        OnInventoryUpdate?.Invoke(this);
        return b;
    }
}
