using UnityEngine;

public class QuestEscort : MonoBehaviour
{
    public HumanAI bot;
    public static QuestEscort instance;

    public void Awake()
    {
        instance = this;
    }
    public void EnableEscort()
    {
        bot.escort = true;
        bot.escortTarget = WorldController.Instance.playerControl.human.gameObject;
    }
    public void DisableEscort()
    {
        bot.escort = false;
        bot.escortTarget = null;
    }
}
