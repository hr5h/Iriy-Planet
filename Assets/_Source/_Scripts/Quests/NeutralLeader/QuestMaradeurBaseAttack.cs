using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class QuestMaradeurBaseAttack : MonoBehaviour
{
    public static QuestMaradeurBaseAttack instance;
    public static string questName;

    public bool attack = false;
    public bool marauderDefeated = false;
    public List<RoutePath> routes = new List<RoutePath>(); //Пути атаки
    public List<Human> humans = new List<Human>(); //Участники атаки
    public List<Human> marauders = new List<Human>();
    public Human leader;

    public void StartQuest() //Начало квеста
    {
        Command.AddTask(questName, "Сообщить главарю о готовности нападать");
        gameObject.SetActive(true);
        humans.RemoveAll(x => !WorldController.Instance.Entities.Humans.Contains(x));
        marauders.RemoveAll(x => !WorldController.Instance.Entities.Humans.Contains(x));
        StartCoroutine(Timeout());
    }
    public void StartAttack() //Начать нападение на мародеров
    {
        if (!attack)
        {
            Logger.Debug("StartAttack");
            attack = true;
            var c = 0;
            if (marauders.Count == 0)
            {
                Logger.Debug("Все мародеры уже мертвы");
                UpdateQuest();
            }
            else
            {
                Command.UpdateTask(questName, "Уничтожить лагерь мародеров в квадрате(-1, -1)");
            }
            for (int j = 0; j < humans.Count; j++)
            {
                if (humans[j] != null)
                {
                    routes[c].SetPath(humans[j].ai as HumanAI);
                    c++;
                    if (c == routes.Count) break;
                }

            }
        }
    }
    public static IEnumerator Timeout()
    {
        yield return Yielders.Get(30);
        instance.StartAttack();
    }
    public void UpdateQuest() //Обновление квеста
    {
        Command.UpdateTask(questName, "Мародеры уничтожены. Поговорить с лидером группировки");
        marauderDefeated = true;
    }
    public void Reward() //Награда за квест
    {
        var player = WorldController.Instance.playerControl.human;
        Command.GiveItem(player.inventory, "ubernit");
        Command.GiveMoney(player, 750);
    }
    public void TraderGiveAmmunition()
    {
        var player = WorldController.Instance.playerControl.human;
        Command.GiveItem(player.inventory, "jacket");
        Command.GiveItem(player.inventory, "ak74");
        Command.GiveItem(player.inventory, "5.45", 60);
        Command.GiveItem(player.inventory, "medkitsmall");

    }
    public void CompleteQuest() //Окончание квеста
    {
        Command.CompleteTask(questName);
        Destroy(gameObject);
    }
    public void Fail(Damageable character = null, Damageable killer = null) //Квест провален
    {
        Command.FailTask(questName);
        Destroy(gameObject);
    }
    private void Awake()
    {
        instance = this;
        questName = "Мародеры";
        gameObject.SetActive(false);
    }
    private void OnMarauderDead(Damageable c, Damageable killer)
    {
        marauders.Remove(c as Human);
        if (marauders.Count == 0)
        {
            UpdateQuest();
        }
    }
    private void Start()
    {
        leader.OnDeath.AddListener(Fail);
        foreach (var x in marauders)
        {
            x.OnDeath.AddListener(OnMarauderDead);
        }
    }
}
