using UnityEngine;

public class QuestPilot : MonoBehaviour
{
    public static QuestPilot instance;
    public static string questName;

    public void StartQuest() //Начало квеста
    {
        Command.AddTask(questName, "Сопроводить выжившего пилота в безопасное место");
        gameObject.SetActive(true);
    }
    public void CompleteQuest() //Окончание квеста
    {
        Command.CompleteTask(questName);
        Destroy(gameObject);
    }
    public void Fail() //Квест провален
    {
        Command.FailTask(questName);
        Destroy(gameObject);
    }
    private void Awake()
    {
        instance = this;
        questName = "Второй пилот";
        gameObject.SetActive(false);
    }

    //private void Start()
    //{
    //    //Тут подписки на разные события и активация объектов
    //}
}
