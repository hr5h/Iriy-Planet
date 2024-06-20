using UnityEngine;

public class QuestGiantAnt : MonoBehaviour
{
    public static QuestGiantAnt instance;
    public static string questName;
    public Monster giant;
    public Human trader;

    public void Reward() //Награда за квест
    {
        var player = WorldController.Instance.playerControl.human;
        Command.GiveMoney(player, 1500);
        Command.CompleteTask(questName);
        Destroy(instance.gameObject); //Квест выполнен, объект больше не нужен
    }
    public void StartQuest()
    {
        instance.gameObject.SetActive(true);
        Command.AddTask(questName, "В районе (-2, -1) бродит гигантский муравей, которого я должен убить по заданию торговца");
    }
    public void Fail(Damageable character, Damageable killer) //Если торговец убит, задание провалено
    {
        Command.FailTask(questName);
        Destroy(instance.gameObject); //Квест провален, объект больше не нужен
    }
    public void UpdateQuest(Damageable character, Damageable killer)
    {
        Command.UpdateTask(questName, "Гигант мертв. Осталось сообщить об этом торговцу");
    }
    private void Awake()
    {
        instance = this;
        questName = "Муравей-гигант";
        gameObject.SetActive(false);
        giant.Awake();
        giant.gameObject.SetActive(false);
    }
    private void Start()
    {
        giant.gameObject.SetActive(true);
        trader.OnDeath.AddListener(Fail); //Подписка на событие смерти торговца, когда он умрет, вызовется метод Fail
        giant.OnDeath.AddListener(UpdateQuest); //Подписка на событие смерти муравья, когда он умрет, задание обновится
    }
}
