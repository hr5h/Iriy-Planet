using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class QuestShipRepair : MonoBehaviour
{
    public static QuestShipRepair instance;
    public static string questName;

    public GameObject repairArea;
    public bool repair = false;
    public float progress = 0;
    public float maxProgress = 100;
    public List<Human> mechanics = new List<Human>();

    public UnityEvent ProgressChanged;

    public void StartQuest()
    {
        Command.AddTask(questName, "Найти способ восстановить поврежденный корабль, чтобы покинуть планету Ирий");
        gameObject.SetActive(true);
    }
    public void CompleteQuest()
    {
        Command.CompleteTask(questName);
        Destroy(gameObject);
        QuestFinal.instance.StartQuest();
    }
    public void StartRepair()
    {
        repair = true;
        repairArea.SetActive(true);
    }
    public void Fail()
    {
        Command.FailTask(questName);
        Destroy(gameObject);
    }
    private void Awake()
    {
        instance = this;
        questName = "Ремонт корабля";
        gameObject.SetActive(false);
    }
    private void MechanicDead(Damageable character, Damageable killer = null)
    {
        mechanics.Remove(character as Human);
    }
    private void Start()
    {
        foreach (var x in mechanics)
        {
            x.OnDeath.AddListener(MechanicDead);
        }
        //подписки на события и активация объектов
    }
}
