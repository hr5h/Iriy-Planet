using UnityEngine;
using Game.Sounds;

public class QuestWhiteFly : MonoBehaviour
{
    public static QuestWhiteFly instance;
    public static string questName;
    public Character whiteFly;
    public Human questor;

    public AudioClip clip;

    public void StartQuest() //Начало квеста
    {
        Command.AddTask(questName, "Чтобы профессор смог доказать одну очень важную теорему, я должен подстрелить белую муху, которая летает в квадрате (-1, 3)");
        AudioPlayer.Instance.PlayDynamicMusic(clip, 1, -1, 2, 5);
        gameObject.SetActive(true);
    }
    public void UpdateQuest(Damageable character = null, Damageable killer = null) //Обновление квеста
    {
        Command.UpdateTask(questName, "Я выполнил задачу. Остается лишь вернуться к профессору");
        AudioPlayer.Instance.StopDynamicMusic(clip);
        Command.ShowMessage("Поздравляем,", "Вы убили муху!", Color.white, 2, true);
    }
    public void Reward() //Награда за квест
    {
        Command.GiveItem(WorldController.Instance.playerControl.human.inventory, "erusBook");
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
        questName = "Белая муха";
        gameObject.SetActive(false);
        whiteFly.Awake();
    }

    private void Start()
    {
        whiteFly.gameObject.SetActive(true);
        questor.OnDeath.AddListener(Fail);
        whiteFly.OnDeath.AddListener(UpdateQuest);
        //questor.OnDead()
        //Тут подписки на разные события и активация объектов
    }
}

