using System.Collections.Generic;
using UnityEngine;

public class QuestNestDestroy : MonoBehaviour
{
    //TODO QuestNestDestroy Второстепенный квест "Муравейники"
    public static QuestNestDestroy instance;
    public static string questName;

    public Human questor;

    public GameObject nestActivator;

    public List<Monster> nests = new List<Monster>();

    public void StartQuest() //Начало квеста
    {
        Command.AddTask(questName, "Один из механиков поручил мне задание по уничтожению муравейников в квадрате (-, -)");
        gameObject.SetActive(true);
    }
    public void UpdateQuest() //Начало квеста
    {
        Command.UpdateTask(questName, "Муравейники уничтожены. Я могу вернуться за наградой");
    }
    public void Reward() //Награда за квест
    {
        //TODO
        var player = WorldController.Instance.playerControl.human;
        Command.GiveMoney(player, 1700);
        Command.CompleteTask(questName);
        Destroy(instance.gameObject);
        CompleteQuest();
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
        questName = "Муравейники";
        gameObject.SetActive(false);
    }

    private void Start()
    {
        questor.OnDeath.AddListener(Fail);
        nestActivator.SetActive(true);
        //Тут подписки на разные события и активация объектов
    }
}
