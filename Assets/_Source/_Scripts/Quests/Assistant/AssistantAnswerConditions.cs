using UnityEngine;

public class AssistantAnswerConditions : MonoBehaviour
{
    public void HasJob() //Профессор может предложить работу
    {
        bool res = !Command.HasTask(QuestFindMineral.questName);
        DialogController.instance.condition = DialogController.instance.condition && res;
    }
    public void NoJob() //У Профессора больше нет работы
    {
        bool res = Command.HasTask(QuestFindMineral.questName);
        DialogController.instance.condition = DialogController.instance.condition && res;
    }
    public void PLayerHasMineral()
    {
        bool res = Command.HasActiveTask(QuestFindMineral.questName) && Command.HasItem(WorldController.Instance.playerControl.human.inventory, "barbonit");
        DialogController.instance.condition = DialogController.instance.condition && res;
    }
}
