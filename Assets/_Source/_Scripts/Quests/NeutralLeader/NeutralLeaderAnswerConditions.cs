using UnityEngine;

public class NeutralLeaderAnswerConditions : MonoBehaviour
{
    public void NoAttackStart() //Атака еще не началась
    {
        bool res = Command.HasTask(QuestMaradeurBaseAttack.questName) && !QuestMaradeurBaseAttack.instance.attack;
        DialogController.instance.condition = DialogController.instance.condition && res;
    }
    public void MaraudersDefeated() //Мародеры уничтожены
    {
        bool res = Command.HasActiveTask(QuestMaradeurBaseAttack.questName) && QuestMaradeurBaseAttack.instance.marauderDefeated;
        DialogController.instance.condition = DialogController.instance.condition && res;
    }
    public void NoMarauderQuest() //Планируется нападение на мародеров
    {
        bool res = !Command.HasTask(QuestMaradeurBaseAttack.questName);
        DialogController.instance.condition = DialogController.instance.condition && res;
    }
}
