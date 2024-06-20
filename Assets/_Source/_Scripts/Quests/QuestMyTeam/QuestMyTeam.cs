using UnityEngine;

public class QuestMyTeam : MonoBehaviour
{
    public static QuestMyTeam instance;
    public static string questName;

    public Human pilot;
    public Human player;

    public void StartQuest() //Начало квеста
    {
        Command.AddTask(questName, "Необходимо узнать, что случилось с пропавшим экипажем");
        gameObject.SetActive(true);
    }
    public void UpdateQuest() //Обновление квеста
    {
        Command.UpdateTask(questName, "Я нашел своего второго пилота. Нужно с ним поговорить");
    }
    public void CompleteQuest() //Окончание квеста
    {
        Command.CompleteTask(questName);
        Destroy(gameObject);
    }

    public void CheckCondition(Inventory inv)
    {
        if (Command.HasItem(inv, "pilotinfo"))
        {
            CompleteQuest();
        }
    }

    public void Fail() //Квест провален
    {
        Command.FailTask(questName);
        Destroy(gameObject);
    }
    private void Awake()
    {
        instance = this;
        questName = "Экипаж";
        player.inventory.OnInventoryUpdate.AddListener(CheckCondition);
        gameObject.SetActive(false);
    }

    //private void Start()
    //{
    //    //Тут подписки на разные события и активация объектов
    //}
}
