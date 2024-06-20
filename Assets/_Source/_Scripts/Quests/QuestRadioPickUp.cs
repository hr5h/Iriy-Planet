using System.Collections;
using UnityEngine;
using Game.Sounds;

public class QuestRadioPickUp : MonoBehaviour
{
    public static QuestRadioPickUp instance;
    public static string questName;
    private Human player;

    public AudioSource audioSource;
    public AudioClip radioClip;

    public void StartQuest() //Начало квеста
    {
        Command.AddTask(questName, "Возле корабля лежит моя рация. Стоит проверить, исправна ли она");
    }
    public void CompleteQuest() //Окончание квеста
    {
        if (Command.HasTask(questName))
        {
            Command.CompleteTask(questName);
        }
    }
    public void Fail() //Квест провален
    {
        Command.FailTask(questName);
        Destroy(gameObject);
    }
    private void Awake()
    {
        instance = this;
        questName = "Поднять рацию";
        GameObject.Find("WorldController").GetComponent<WorldController>().playerControl.human.inventory.OnInventoryUpdate.AddListener(CheckCondition);
        gameObject.SetActive(false);
    }

    public void CheckCondition(Inventory inv)
    {
        gameObject.SetActive(true);
        if (Command.HasItem(inv, "portativeradio", 1))
        {
            AudioPlayer.Instance.PlaySoundFX(radioClip, transform.position);
            CompleteQuest();
            inv.OnInventoryUpdate.RemoveListener(CheckCondition);
            StartCoroutine(StartNeutralQuest());
        }
    }

    public static IEnumerator StartNeutralQuest()
    {
        yield return Yielders.Get(1);
        if (!Scenario.instance.firstBlackout)
        {
            if (!WorldController.Instance.blackout)
            {
                Command.ShowMessage("Неизвестный источник:", "Внимание, приближается затмение! Все, кто находится вне укрытия, может переждать затмение у нас в лагере. Наши координаты (-2, 0)", Color.white);
            }
            else
            {
                Command.ShowMessage("Неизвестный источник:", "Затмение уже идет! Все, кто находится вне укрытия, может переждать затмение у нас в лагере. Наши координаты (-2, 0)", Color.white);
            }
            yield return Yielders.Get(3);
            QuestToNeutralBase.instance.StartQuest();
        }
        else
        {
            Command.ShowMessage("Игрок:", "Никто не отвечает...", Color.cyan);
        }
        Destroy(instance.gameObject);
    }

    //private void Start()
    //{
    //    //Тут подписки на разные события и активация объектов
    //}
}
