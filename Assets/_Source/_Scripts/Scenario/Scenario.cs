using System.Collections;
using UnityEngine;
using Game.Sounds;

//Сценарный объект, однократно выполняющий действия на старте игры
public class Scenario : MonoBehaviour
{
    public static Scenario instance;
    public float time;
    public bool firstBlackout = false;

    public GameObject WaspGroup0;
    public GameObject WaspGroup1;
    public GameObject WaspGroup2;
    public GameObject NeutralBase;

    public Human player;

    public AudioClip clip;

    private void Awake()
    {
        instance = this;
    }
    private void FirstBlackoutBegin()
    {
        WaspGroup0.transform.position = NeutralBase.transform.position;
        WaspGroup1.transform.position = NeutralBase.transform.position;
        WaspGroup2.transform.position = NeutralBase.transform.position;
        Blackout.instance.OnBlackoutBegin.RemoveListener(FirstBlackoutBegin);
        AudioPlayer.Instance.PlayDynamicMusic(clip, 1, -1, 2, 10);
    }
    private void FirstBlackoutEnd()
    {
        firstBlackout = true;
        Blackout.instance.OnBlackoutEnd.RemoveListener(FirstBlackoutEnd);
        if (Command.HasActiveTask(QuestToNeutralBase.questName))
        {
            QuestToNeutralBase.instance.CancelQuest(); //Если игрок получил квест, но не выполнил до конца первого затмения, то отменить квест
        }
        else
        {
            Destroy(QuestToNeutralBase.instance); //Квест не начнется после первого затмения
        }
        if (Command.HasActiveTask(QuestNeutralBaseDefence.questName))
        {
            QuestNeutralBaseDefence.instance.UpdateQuest();
        }
        else
        {
            Destroy(QuestNeutralBaseDefence.instance);
        }
    }
    private void PlayerChangeClan() //Изменить группировку игрока при надевании брони
    {
        if (player.armorData && (player.armorData.Title == "Костюм сумасшедшего"))
        {
            player.clan = "Сумасшедшие";
            player.body.ChangeSkin(player.body.MadmanSkin);
        }
        else
        {
            player.clan = "Одиночки";
            player.body.ChangeSkin(player.body.DefaultSkin);
        }
    }
    void Start()
    {
        player = WorldController.Instance.playerControl.human;
        player.OnArmorEquip.AddListener(PlayerChangeClan);
        player.OnArmorUnEquip.AddListener(PlayerChangeClan);
        Blackout.instance.OnBlackoutEnd.AddListener(FirstBlackoutEnd);
        Blackout.instance.OnBlackoutBegin.AddListener(FirstBlackoutBegin);
        StartCoroutine(StartIntroduction());
        //for (int i = 0; i<2; i++)
        //{ 
        //    Vector2 pos = new Vector2(500, i*70);
        //    var npc = Command.SpawnNPC(pos, "Одиночки");
        //    npc.canSpeak = true;
        //    npc.canTrade = true;
        //    Command.ChangeName(npc);
        //    Command.RandomWeapon(npc);
        //    npc.EquipArmor(Command.items["armorjacket"] as ArmorData, (Command.items["armorjacket"] as ArmorData).conditionMax);
        //    npc.money = 250 + 5 * Random.Range(0, 750);
        //}
        //for (int i = 0; i < 2; i++)
        //{
        //    Vector2 pos = new Vector2(-500, i * 70);
        //    var npc = Command.SpawnNPC(pos, "Мародеры");
        //    Command.ChangeName(npc);
        //    Command.RandomWeapon(npc);
        //    npc.EquipArmor(Command.items["armorjacket"] as ArmorData, (Command.items["armorjacket"] as ArmorData).conditionMax);
        //}

        //Command.AddTask("Ремонт корабля","Найти способ восстановить поврежденный корабль, чтобы покинуть планету Ирий");
        //Command.AddTask("Открыть инвентарь", "Найти клавишу, с помощью которой открывается инвентарь главного героя. Крайне сложное задание, мало кому удается его выполнить");
        //Destroy(gameObject);
    }
    public static IEnumerator StartIntroduction()
    {
        yield return Yielders.Get(1);
        //QuestFinal.instance.StartQuest();
        Command.ShowMessage("Игрок:", "...", Color.cyan);
        yield return Yielders.Get(3);
        Command.ShowMessage("Игрок:", "Что произошло? Что с моим кораблем и где вся моя команда?", Color.cyan);
        yield return Yielders.Get(3);
        QuestShipRepair.instance.StartQuest();
        QuestMyTeam.instance.StartQuest();
        yield return Yielders.Get(3);
        if (!Command.HasItem(WorldController.Instance.playerControl.human.inventory, "portativeradio"))
        {
            Command.ShowMessage("Игрок:", "На земле лежит рация, надо проверить, работает ли она...", Color.cyan);
            yield return Yielders.Get(3);
            if (!Command.HasItem(WorldController.Instance.playerControl.human.inventory, "portativeradio"))
            {
                QuestRadioPickUp.instance.StartQuest();
            }
        }
    }
}
