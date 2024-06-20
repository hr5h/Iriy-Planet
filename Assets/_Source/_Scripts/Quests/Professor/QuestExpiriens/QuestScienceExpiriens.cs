using Game.Sounds;
using UnityEngine;

public class QuestScienceExpiriens : MonoBehaviour
{
    public static QuestScienceExpiriens instance;
    public static string questName;
    public bool measuringComplete;
    public MonsterPopulation spiders;
    public Human questor;
    private Human player;

    public AudioClip clip;

    public void StartQuest() //Начало квеста
    {
        Command.AddTask(questName, "Профессор сказал провести замеры в глубине Синего Леса с помощью специального прибора");
        Command.GiveItem(WorldController.Instance.playerControl.human.inventory, "professorDevice");
        gameObject.SetActive(true);
    }
    public void UpdateQuest() //Обновление квеста
    {
        Command.UpdateTask(questName, "Вернуться к ученому с результатами");
        AudioPlayer.Instance.PlayDynamicMusic(clip, 1, -1, 3, 5);
    }
    public void Measuring()
    {
        measuringComplete = true;
        UpdateQuest();
        foreach (var x in spiders.monsters)
        {
            (x.ai as MonsterAI).isAgressive = true;
            (x.ai as MonsterAI).target = WorldController.Instance.playerControl.human;
            (x.ai as MonsterAI).endlessHunting = true;
        }
    }

    public void Reward() //Награда за квест
    {

    }
    public void CompleteQuest() //Окончание квеста
    {
        Command.CompleteTask(questName);
        Command.RemoveItem(WorldController.Instance.playerControl.human.inventory, "professorDevice");
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
        questName = "Исследование";
        gameObject.SetActive(false);
    }

    private void Start()
    {
        questor.OnDeath.AddListener(Fail);
        //Тут подписки на разные события и активация объектов
    }
}
