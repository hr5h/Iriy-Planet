using UnityEngine;

public class QuestDravings : MonoBehaviour
{
    public static QuestDravings instance;
    public static string questName;
    public Human questor;

    public Human mad; //Сумасшедший, который появляется после начала квеста
    public RoutePath route; //Маршрут сумасшедшего
    public Weapon weapon; //Оружие сумасшедшего

    public void StartQuest() //Начало квеста
    {
        Command.AddTask(questName, "Лидер механиков сказал, что корабль невозможно починить без конструкционных чертежей. Сейчас чертежи находятся в квадрате (2, 2) на базе враждебной группировки - \"Сумасшедших\". Нужно найти способ заполучить эти чертежи");
        gameObject.SetActive(true);
        EnableMadman();
    }
    public void MadmanDead(Damageable character = null, Damageable killet = null)
    {
        mad.inventory.Add(mad.UnEquipArmor());
    }
    public void EnableMadman()
    {
        weapon.gameObject.SetActive(true);
        mad.gameObject.SetActive(true);
        route.SetPath(mad.ai as HumanAI);
    }
    public void UpdateQuest() //Начало квеста
    {
        Command.UpdateTask(questName, "Конструкционные чертежи у меня, теперь я должен отнести их механикам");
    }
    public void CompleteQuest() //Окончание квеста
    {
        Command.CompleteTask(questName);
        QuestShipRepair.instance.StartRepair();
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
        questName = "Чертежи";
        mad.Awake();
        gameObject.SetActive(false);
    }

    private void Start()
    {
        questor.OnDeath.AddListener(Fail);
        mad.gameObject.SetActive(true);
        mad.OnDeath.AddListener(MadmanDead);
        //Тут подписки на разные события и активация объектов
    }
}
