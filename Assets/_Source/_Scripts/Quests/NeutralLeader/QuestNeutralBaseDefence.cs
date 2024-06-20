using UnityEngine;

public class QuestNeutralBaseDefence : MonoBehaviour
{
    public static QuestNeutralBaseDefence instance;
    public static string questName;
    public Human neutralLeader;

    public void StartQuest() //Начало квеста
    {
        Command.AddTask(questName, "Лагерь, в который я пришел, атакован инопланетянами. Нужно помочь его защитить");
        gameObject.SetActive(true);
    }
    public void UpdateQuest() //Начало квеста
    {
        Command.UpdateTask(questName, "Лагерь в безопасности. Теперь можно поговорить с главным");
        neutralLeader.canSpeak = true;
    }
    public void Reward() //Награда за квест
    {

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
        questName = "Защита лагеря";
        gameObject.SetActive(false);
    }

    private void Start()
    {
        neutralLeader.OnDeath.AddListener(Fail); //Если главарь умирает, квест провален
    }
}
