using UnityEngine;

public class NeutralTraderAnswerConditions : MonoBehaviour
{
    public void GiantIsDead() //Взял ли игрок задание на уничтожение муравья гиганта
    {
        bool res = Command.HasTask("Муравей-гигант") && (QuestGiantAnt.instance.giant == null);
        DialogController.instance.condition = DialogController.instance.condition && res;
    }
    public void HasJob() //Торговец может предложить работу
    {
        bool res = !Command.HasTask("Муравей-гигант");
        DialogController.instance.condition = DialogController.instance.condition && res;
    }
    public void NoJob() //У торговца больше нет работы
    {
        bool res = Command.HasTask("Муравей-гигант");
        DialogController.instance.condition = DialogController.instance.condition && res;
    }
    public void MarauderBaseAttack() //Планируется нападение на мародеров
    {
        bool res = Command.HasActiveTask(QuestMaradeurBaseAttack.questName);
        DialogController.instance.condition = DialogController.instance.condition && res;
    }
}
