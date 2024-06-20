using System.Collections;
using UnityEngine;

public class QuestProtector : MonoBehaviour
{
    public static QuestProtector instance;
    public static string questName;

    public bool protectorComplete;
    public Human questor;


    public void StartQuest() //Начало квеста
    {
        Command.AddTask(questName, "Профессор сказал, что работает над чем-то важным. Нужно дождаться, когда он закончит работу");
        gameObject.SetActive(true);
        StartCoroutine(Timer());
    }
    public void UpdateQuest() //Обновление квеста
    {
        Command.UpdateTask(questName, "Пришло время поговорить с профессором по поводу изобретения");
    }
    public void Reward() //Награда за квест
    {
        Command.GiveItem(WorldController.Instance.playerControl.human.inventory, "protector", 2);
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
        questName = "Новое изобретение";
        gameObject.SetActive(false);
    }

    public static IEnumerator Timer()
    {
        yield return Yielders.Get(60);
        instance.UpdateQuest();
        instance.protectorComplete = true;
    }

    private void Start()
    {
        questor.OnDeath.AddListener(Fail);
        //Тут подписки на разные события и активация объектов
    }
}
