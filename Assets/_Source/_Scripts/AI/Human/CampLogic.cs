using System.Collections.Generic;
using UnityEngine;

public class CampLogic : MonoBehaviour
{
    //Логика для лагерей
    public CampArea campArea;
    public string clan; //Какой группировке принадлежит лагерь
    public List<Human> humans = new List<Human>(); //Люди в лагере
    public List<PatrolPath> patrols = new List<PatrolPath>(); //Все точки патрулирования в лагере
    public List<HoldPos> positions = new List<HoldPos>(); //Все точки позиционирования в лагере
    public bool destroy;

    private void Start()
    {
        campArea.OnHumanEnter.AddListener(AddHuman);
        campArea.OnDead.AddListener(CampDestroy);
        clan = "free";
    }
    public void CampDestroy() //Удаление лагеря
    {
        destroy = true;
        for (int i = 0; i < humans.Count; i++)
        {
            RemoveHuman(humans[i]);
        }
        Destroy(gameObject);
    }
    public void RemoveHuman(Damageable h, Damageable killer = null)
    {
        RemoveHuman(h as Human);
    }
    public void RemoveHuman(Human h) //Если человек покидает базу, удаляем из списка и отменяем его задачи
    {
        if (!destroy)
        {
            h.OnDamage.RemoveListener(Defence);
            h.OnDeath.RemoveListener(RemoveHuman);
            //print("remove");
            humans.Remove(h);
        }
        var ind = patrols.FindIndex(x => x.ai == h.ai);
        if (ind >= 0)
        {
            patrols[ind].RemovePath();
            if (!destroy)
            {
                foreach (var x in humans) //Поручить задачи удаляемого человека другому
                {
                    if (IsFreeAI(x.ai as HumanAI))
                    {
                        patrols[ind].SetPath(x.ai as HumanAI);
                        break;
                    }
                }
            }
        }
        ind = positions.FindIndex(x => x.ai == h.ai);
        if (ind >= 0)
        {
            positions[ind].RemoveHoldPosition();
            if (!destroy)
            {
                foreach (var x in humans) //Поручить задачи удаляемого человека другому
                {
                    if (IsFreeAI(x.ai as HumanAI))
                    {
                        patrols[ind].SetPath(x.ai as HumanAI);
                        break;
                    }
                }
            }
        }
        if (humans.Count == 0) //Если в списке не осталось людей
        {
            clan = "free";
            CheckHumans();
        }
    }

    public bool IsFreeAI(HumanAI ai) //Проверка, занят ли чем-нибудь бот
    {
        return (!ai.patrol && !ai.route && !ai.holdPosition && !ai.escort && !ai.owner.questPerson);
    }
    public void FindJob(HumanAI ai) //Найти задачу для НПС
    {
        if (IsFreeAI(ai)) //Если он ничем не занят
        {
            foreach (var x in patrols)
            {
                if (x.ai == null)
                {
                    x.SetPath(ai);
                    break;
                }
            }
            foreach (var x in positions)
            {
                if (x.ai == null)
                {
                    x.SetHoldPosition(ai);
                    break;
                }
            }
        }
    }

    public void CheckHumans() //Проверить всех людей у костра
    {
        foreach (var h in campArea.humans)
        {
            AddHuman(h);
        }
    }

    public void Defence(Damageable defender, Damageable target) //Защита базы при нападении
    {
        foreach (var x in humans)
        {
            if (x != null)
            {
                //print(x.name + " => " + target.name);
                x.ai.SetTarget((Character)target);
            }
        }
    }

    public void AddHuman(Human h) //Если в лагерь попал новый человек, то добавляем его в список и находим ему задачу
    {
        if (h.ai) //Рассматриваем только НПС
        {
            if (!h.IsDead && !humans.Contains(h)) //Если такого человека еще нет в списке
            {
                h.OnDeath.AddListener(RemoveHuman);
                h.OnDamage.AddListener(Defence);
                if (humans.Count == 0)
                    clan = h.clan;
                humans.Add(h);
                var ai = h.ai as HumanAI;
                if (ai.owner.relation[clan] > 0) //Если он нейтрален к группировке, которая владеет базой
                {
                    FindJob(ai);
                }
            }
        }
    }
}
