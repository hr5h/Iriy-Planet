using System.Collections;
using UnityEngine;

public class QuestToNeutralBase : MonoBehaviour
{
    public static QuestToNeutralBase instance;
    public static string questName;

    public GameObject questCollider;
    public GameObject player;

    public void StartQuest() //Начало квеста
    {
        Command.AddTask(questName, "На приемник пришло предупреждение об опасности. Неизвестный советовал добраться до лагеря в квадрате (-2, 0)");
        gameObject.SetActive(true);
    }
    public void CompleteQuest() //Окончание квеста
    {
        Command.CompleteTask(questName);
    }
    public void CancelQuest() //Отмена квеста
    {
        Command.CancelTask(questName);
        Destroy(gameObject);
    }
    public void Fail() //Квест провален
    {
        Command.FailTask(questName);
        Destroy(gameObject);
    }
    public static IEnumerator StartNeutralReplic()
    {
        if (Command.HasActiveTask(questName))
        {
            instance.CompleteQuest();
        }
        if (QuestNeutralBaseDefence.instance.neutralLeader)
        {
            yield return Yielders.Get(1);
            Command.ShowMessage("Лидер группировки:", "Путник, иди сюда, к костру", Color.white);
            yield return Yielders.Get(3);
            if (QuestNeutralBaseDefence.instance.neutralLeader)
            {
                QuestNeutralBaseDefence.instance.StartQuest();
            }
        }
        Destroy(instance);
    }
    private void Awake()
    {
        instance = this;
        questName = "Добраться до лагеря";
    }

    //private void Start()
    //{
    //    //Тут подписки на разные события и активация объектов
    //}
}
