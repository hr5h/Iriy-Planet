using UnityEngine;

public class QuestFindMineral : MonoBehaviour
{
    public static QuestFindMineral instance;
    public static string questName;
    public Human questor;
    public void StartQuest() //Начало квеста
    {
        Command.AddTask(questName, "Найти и принести ассистенту минерал \"Барбонит\"");
        gameObject.SetActive(true);
    }
    public void Reward() //Награда за квест
    {
        if (Command.HasItem(WorldController.Instance.playerControl.human.inventory, "barbonit"))
        {
            Command.GiveMoney(WorldController.Instance.playerControl.human, 5000);
            Command.RemoveItem(WorldController.Instance.playerControl.human.inventory, "barbonit");
        }
        CompleteQuest();
    }
    public void CompleteQuest() //Окончание квеста
    {
        Command.CompleteTask(questName);
        Destroy(gameObject);
    }
    public void Fail(Damageable character, Damageable killer) //Квест провален
    {
        Command.FailTask(questName);
        Destroy(gameObject);
    }
    private void Awake()
    {
        instance = this;
        questName = "Найти минерал";
        gameObject.SetActive(false);
    }

    private void Start()
    {
        questor.OnDeath.AddListener(Fail);
        //Тут подписки на разные события и активация объектов
    }
}
