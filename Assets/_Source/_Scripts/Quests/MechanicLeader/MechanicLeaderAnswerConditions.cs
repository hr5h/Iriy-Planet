using UnityEngine;

public class MechanicLeaderAnswerConditions : MonoBehaviour
{
    public void NoTask() //»грок не получил задани¤
    {
        bool res = !Command.HasTask(QuestDravings.questName);
        DialogController.instance.condition = DialogController.instance.condition && res;
    }
    public void PlayerHasDravings()
    {
        bool res = Command.HasActiveTask(QuestDravings.questName) && Command.HasItem(WorldController.Instance.playerControl.human.inventory, "drawings");
        DialogController.instance.condition = DialogController.instance.condition && res;
    }
    public void MechanicsIsDead()
    {
        bool res = QuestShipRepair.instance.repair && QuestShipRepair.instance.mechanics.Count == 0;
        DialogController.instance.condition = DialogController.instance.condition && res;
    }
}
