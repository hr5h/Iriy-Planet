using System.Collections.Generic;
using UnityEngine;

//Этот скрипт отвечает за предметы, которые выдаются персонажам в начале игры
public class StartItems : MonoBehaviour
{
    public List<string> items = new List<string>();
    private Inventory inventory;

    private void Start()
    {
        inventory = GetComponent<Inventory>();
        foreach (var x in items)
        {
            var item = x.ToLower();
            //print(inventory.owner.MyName + " " + item);
            if (Command.items[item] is AmmoData)
            {
                Command.GiveItem(inventory, item, (Command.items[item] as AmmoData).ammoCount);
            }
            else
            {
                Command.GiveItem(inventory, item, 1);
            }
        }
    }
}
