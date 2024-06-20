using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/// <summary>
/// Класс для оптимизации обновления объектов
/// </summary>
public class UpdateManager : MonoBehaviour
{
    private const int MaxUpdatables = 1000;
    private IUpdatable[] updatables = new IUpdatable[MaxUpdatables];
    private int count = 0;

    public void Add(IUpdatable item)
    {
        updatables[count] = item;
        count++;
    }

    public void Remove(IUpdatable item)
    {
        for (int i = 0; i < count; i++)
        {
            if (updatables[i] == item)
            {
                updatables[i] = updatables[count - 1];
                updatables[count - 1] = null;
                count--;
                return;
            }
        }
    }

    private void Update()
    {
        for (int i = 0; i < count; i++)
        {
            updatables[i].ManualUpdate();
        }
        //Logger.Debug(count.ToString());
    }
}
