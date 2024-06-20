using System.Collections;
using UnityEngine;

public class QuestRadio : MonoBehaviour
{
    public GameObject checkMonstersArea;

    public static QuestRadio instance;
    public static string questName;
    public Human questTrader;
    //public static UnityEvent questStart;
    public static void Reward() //Награда за квест
    {
        var player = WorldController.Instance.playerControl.human;
        Command.GiveItem(player.inventory, "irium");
        Command.GiveMoney(player, 1500);
        Command.CompleteTask(questName);
        Destroy(instance.gameObject); //Квест выполнен, объект больше не нужен
    }
    public void StartQuest()
    {
        instance.gameObject.SetActive(true);
        if (instance.questTrader == null) //Игрок слишком поздно поднял рацию, торговец мертв
        {
            Destroy(instance.gameObject);
        }
        else
        {
            instance.StartCoroutine(StartChatBox());
        }
    }
    public void UpdateQuest() //Обновление задания при уничтожении всех муравьев
    {
        Command.UpdateTask(questName, "Теперь торговец в безопасности, самое время с ним поговорить");
        instance.questTrader.canSpeak = true;
        instance.questTrader.canTrade = true;
    }
    public void Fail(Damageable character, Damageable killer) //Если торговец убит, задание провалено
    {
        Command.FailTask(questName);
        Destroy(instance.gameObject); //Квест провален, объект больше не нужен
    }

    public void CheckCondition(Inventory inv) //Проверка наличия рации у игрока
    {
        if (Command.HasItem(inv, "portativeradio"))
        {
            instance.StartQuest(); //Начать квест
            inv.OnInventoryUpdate.RemoveListener(CheckCondition);
            WorldController.Instance.playerControl.human.inventory.OnInventoryUpdate.RemoveListener(CheckCondition); //Отписываемся от события, больше этот метод вызываться не будет
        }
    }

    private void Awake()
    {
        instance = this;
        questName = "Спасение торговца";
        //WARNING WorldController.instance не работает в Awake
        GameObject.Find("WorldController").GetComponent<WorldController>().playerControl.human.inventory.OnInventoryUpdate.AddListener(CheckCondition); //Когда инвентарь игрока обновляется, мы проверяем квестовое условие
        gameObject.SetActive(false);
    }
    private void Start()
    {
        questTrader.OnDeath.AddListener(Fail); //Подписка на событие смерти торговца, когда он умрет, вызовется метод Fail
        checkMonstersArea.SetActive(true);
    }
    public static IEnumerator StartChatBox()
    {
        yield return Yielders.Get(1);
        Command.ShowMessage("Неизвестный источник:", "Помогите!", Color.white);
        yield return Yielders.Get(3);
        Command.ShowMessage("Неизвестный источник:", "Помогите, меня окружили муравьи!", Color.white);
        yield return Yielders.Get(3);
        Command.ShowMessage("Неизвестный источник:", "Пожалуйста, помогите, я нахожусь на координатах (0,2).", Color.white);
        yield return Yielders.Get(4);
        Command.AddTask(questName, "Моя портативная рация поймала сигнал с призывом о помощи. Человек сказал, что находится в  Квадрате(2; 0)");
    }
}
