using Game.Sounds;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class QuestSabotage : MonoBehaviour
{
    public static QuestSabotage instance;
    public static string questName;
    public GameObject bombPref;
    public List<Human> madmans = new List<Human>(); //Все сумасшедшие на базе
    public Human player;
    public Human leader;
    public RoutePath route;
    public GameObject bonfire;
    public GameObject bomb;
    public bool leaderIsGone;
    public bool blackoutEnd;
    public bool started;
    public bool bombPlanted;

    public AudioSource audioSource;
    public AudioClip bombClip;

    public AudioClip clip;

    public UnityEvent OnLeaderMove;
    public void StartQuest() //Начало квеста
    {
        leader.canSpeak = true;
        Command.AddTask(questName, "У меня есть костюм сумасшедших и взрывчатка. Кажется, с этим можно что-то придумать...");
        gameObject.SetActive(true);
    }
    public void PlayerInBase()
    {
        started = true;
        UpdateQuest1();
    }
    public void BombPlant() //Установка бомбы
    {
        bombPlanted = true;
        UpdateQuest3();
        Command.RemoveItem(player.inventory, "bomb");
        bomb = Instantiate(bombPref, player.transform.position, player.transform.rotation);
        AudioPlayer.Instance.PlayDynamicMusic(clip, 1, -1, 3, 5);
    }
    public void LeaderMove()
    {
        (leader.ai as HumanAI).holdPosition = false;
        route.SetPath(leader.ai as HumanAI);
        leaderIsGone = true;
        leader.canSpeak = false;
        UpdateQuest2();
        OnLeaderMove?.Invoke();
    }
    public void BombExplosion()
    {
        AudioPlayer.Instance.PlaySoundFX(bombClip, bomb.transform.position);
        //TODO сделать нормальный взрыв
        //GameObject bombExplosion = Instantiate(Resources.Load<GameObject>("Prefabs/BombExplosion"), new Vector3(bomb.transform.position.x, bomb.transform.position.y, bomb.transform.position.z), Quaternion.identity);
        CompleteQuest();
        //UpdateQuest4();
        Destroy(bomb);
        Destroy(bonfire);
        Blackout.instance.OnBlackoutEnd.AddListener(OnBlackoutEnd);
        foreach (var x in madmans)
        {
            x.Death();
        }
    }
    public void OnBlackoutEnd()
    {
        Blackout.instance.OnBlackoutEnd.RemoveListener(OnBlackoutEnd);
        blackoutEnd = true;
        CompleteQuest();
    }
    public void UpdateQuest1() //Обновление квеста
    {
        Command.UpdateTask(questName, "Отвлечь лидера группировки, чтобы незаметно установить взрывное устройство на базе");
    }
    public void UpdateQuest2() //Обновление квеста
    {
        Command.UpdateTask(questName, "Заложить взрывчатку у костра");
    }
    public void UpdateQuest3() //Обновление квеста
    {
        Command.UpdateTask(questName, "Взрывчатка установлена, пора покидать базу");
    }
    public void UpdateQuest4() //Обновление квеста
    {
        Command.UpdateTask(questName, "Костер на базе сумасшедших взорван. Осталось дождаться затмения");
        AudioPlayer.Instance.StopDynamicMusic(clip);
    }
    public void CheckCondition() //Проверка, есть ли у игрока взрывчатка и броня сумасшедших
    {
        if (!Command.HasTask(questName))
        {
            if (((Command.HasItem(player.inventory, "madmancostume"))
                || ((player.armorData && player.armorData.Title == "Костюм сумасшедшего")))
                && Command.HasItem(player.inventory, "bomb"))
            {
                StartQuest();
                player.OnArmorEquip.RemoveListener(CheckCondition);
                player.inventory.OnInventoryUpdate.RemoveListener(CheckCondition);
            }
        }
    }
    public void CheckCondition(Inventory inv)
    {
        CheckCondition();
    }
    public void Reward() //Награда за квест
    {

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
        questName = "Диверсия";
        player.OnArmorEquip.AddListener(CheckCondition);
        player.inventory.OnInventoryUpdate.AddListener(CheckCondition);
        gameObject.SetActive(false);
    }

    private void Start()
    {
        player = WorldController.Instance.playerControl.human;
        player.OnArmorEquip.AddListener(CheckCondition);
        player.inventory.OnInventoryUpdate.AddListener(CheckCondition);
        CheckCondition();
        //Тут подписки на разные события и активация объектов
    }
}
