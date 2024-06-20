using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class TraderAI : MonoBehaviour, IUpdatable
{
    public Human owner;
    public GameObject player;

    //const int AssSize = 3;
    enum AssortmentSize { Small, Medium, Large } //Насколько большой ассортимент у торговца

    const int Specs = 5;
    enum Specialize { Mineral, Ammo, Medicine, Weapon, Armor } //На чем специализируется торговец

    List<string> Names = new List<string>();

    [SerializeField]
    AssortmentSize assortmentSize;
    [SerializeField]
    List<Specialize> specializes = new List<Specialize>();

    public void ClearInventory() //Очищение инвентаря
    {
        for (int i = 0; i < owner.inventory.items.Count; i++)
        {
            if (!owner.inventory.items[i].data.questItem) //Квестовые предметы не удаляем
                owner.inventory.Remove(i);
        }
    }
    public void Assortment() //Обновление ассортимента
    {
        //var random = new System.Random();
        //specializes = new List<Specialize>();
        //assortmentSize = (AssortmentSize)Random.Range(0, AssSize);
        //for (int i = 0; i < Specs; i++)
        //{
        //    if (1.0 / (i + 2) >= Random.Range(0.0f, 1.0f))
        //        specializes.Add((Specialize)i);
        //}

        if (specializes.Count == 0)
        {
            specializes.Add((Specialize)Random.Range(0, Specs));
            //return;
        }
        int itemCount = 0;
        switch (assortmentSize)
        {
            case AssortmentSize.Small:
                itemCount = 5;
                break;
            case AssortmentSize.Medium:
                itemCount = 10;
                break;
            case AssortmentSize.Large:
                itemCount = 15;
                break;
        }
        itemCount += Random.Range(0, 5);
        itemCount /= specializes.Count;
        //Не добавлять квестовые предметы
        foreach (Specialize s in specializes)
        {
            Names.Clear();
            switch (s)
            {
                case Specialize.Weapon:
                    foreach (var data in Command.weapons.Values.Where(x => x.Icon != null && !x.questItem))     //Обход массива ассетов, которые неквестовые и имеют иконку
                        Names.Add(data.name.ToLower());                                                                                         //Сбор названий
                    for (int i = 0; i < itemCount; i++)
                    {
                        var temp = Random.Range(0, Names.Count);                                                                                //Случайное название
                        //var tempCount = Random.Range(0, itemCount);
                        Command.GiveItem(owner.inventory, Names[temp]);                                                                         //Кладём случайный предмет в инвентарь торговца
                        Names.RemoveAt(temp);                                                                                                   //Убираем это название, чтобы не повторяться
                        //i += tempCount;
                        //if (Names.Count >= i) Names.RemoveAt(temp);
                        //i += i - 1;
                        if (Names.Count == 0) break;
                    }
                    break;
                case Specialize.Armor:
                    foreach (var data in Command.armors.Values.Where(x => x.Icon != null && !x.questItem))
                        Names.Add(data.name.ToLower());
                    for (int i = 0; i < Random.Range(1, itemCount); i++)
                    {
                        var temp = Random.Range(0, Names.Count);
                        //var tempCount = Random.Range(0, itemCount);
                        Command.GiveItem(owner.inventory, Names[temp]);
                        Names.RemoveAt(temp);
                        //i += tempCount;
                        //if (Names.Count >= i) Names.RemoveAt(temp); //На случай, если названий меньше, чем ItemCount
                        //i += i - 1;
                        if (Names.Count == 0) break;
                    }
                    break;
                case Specialize.Medicine:
                    foreach (var data in Command.medicals.Values.Where(x => x.Icon != null && !x.questItem))
                        Names.Add(data.name.ToLower());

                    for (int i = 0; i < itemCount; i++)
                    {
                        var temp = Random.Range(0, Names.Count);
                        var tempCount = Random.Range(1, (Command.items[Names[temp]] as CollectableItemData).countMax + 1);
                        Command.GiveItem(owner.inventory, Names[temp], tempCount);
                        //i += tempCount;
                        //if (Names.Count >= i) Names.RemoveAt(temp);
                        //i += i - 1;
                        if (Names.Count == 0) break;
                    }
                    break;
                case Specialize.Mineral:
                    foreach (var data in Command.minerals.Values.Where(x => x.Icon != null && !x.questItem))
                        Names.Add(data.name.ToLower());
                    for (int i = 0; i < itemCount; i++)
                    {
                        var temp = Random.Range(0, Names.Count);
                        Command.GiveItem(owner.inventory, Names[temp]);
                        Names.RemoveAt(temp);
                        if (Names.Count == 0) break;
                    }
                    break;
                case Specialize.Ammo:
                    foreach (var data in Command.ammo.Values.Where(x => x.Icon != null && !x.questItem))
                        Names.Add(data.name.ToLower());
                    for (int i = 0; i < itemCount; i++)
                    {

                        var temp = Random.Range(0, Names.Count);
                        Command.GiveItem(owner.inventory, Names[temp], (Command.items[Names[temp]] as CollectableItemData).countMax);
                        //if (Names.Count >= i) Names.RemoveAt(temp);
                        if (Names.Count == 0) break;
                    }
                    break;
            }
        }
    }
    public void UpdateAssortment()
    {
        Command.GiveItem(owner.inventory, "armorjacket");
        Command.GiveItem(owner.inventory, "jacket");
        Command.GiveItem(owner.inventory, "medkitmedium", 6);
        Command.GiveItem(owner.inventory, "medkitsmall", 9);
        Command.GiveItem(owner.inventory, "barbonit");
        Command.GiveItem(owner.inventory, "ak74");
        Command.GiveItem(owner.inventory, "uzi");
        Command.GiveItem(owner.inventory, "pistol");
        Command.GiveItem(owner.inventory, "5.45", 500);
        Command.GiveItem(owner.inventory, "9mm", 400);
    }
    public void Start()
    {
        owner = GetComponent<Human>();
        player = WorldController.Instance.playerControl.human.gameObject;
        Assortment();
        owner.money = 35000 + Random.Range(0, 5000) * 5;
        owner.DefaultValues("Торговцы");
    }
    private void OnEnable() => StartCoroutine(OnEnableCoroutine());
    private IEnumerator OnEnableCoroutine()
    {
        yield return null;
        WorldController.Instance.Updater.Add(this);
    }
    private void OnDisable()
    {
        WorldController.Instance.Updater.Remove(this);
    }
    public void ManualUpdate()
    {
        if (player != null)
        {
            transform.rotation = Quaternion.FromToRotation(Vector3.right, player.transform.position - transform.position);
        }
    }
}
