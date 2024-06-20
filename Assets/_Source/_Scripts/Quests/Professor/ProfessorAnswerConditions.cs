using UnityEngine;

public class ProfessorAnswerConditions : MonoBehaviour
{
    public void FlyIsDead() //Взял ли игрок задание на уничтожение муравья гиганта
    {
        bool res = Command.HasTask(QuestWhiteFly.questName) && (QuestWhiteFly.instance.whiteFly == null);
        DialogController.instance.condition = DialogController.instance.condition && res;
    }
    public void HasJob() //Профессор может предложить работу
    {
        bool res = !Command.HasTask(QuestWhiteFly.questName);
        DialogController.instance.condition = DialogController.instance.condition && res;
    }
    public void NoJob() //У Профессора больше нет работы
    {
        bool res = Command.HasTask(QuestWhiteFly.questName);
        DialogController.instance.condition = DialogController.instance.condition && res;
    }
    public void NoExpirienceStart() //Квест с замерами еще не начат
    {
        bool res = !Command.HasTask(QuestScienceExpiriens.questName);
        DialogController.instance.condition = DialogController.instance.condition && res;
    }
    public void MeasureComplete() //Замеры произведены
    {
        bool res = Command.HasActiveTask(QuestScienceExpiriens.questName) && QuestScienceExpiriens.instance.measuringComplete;
        DialogController.instance.condition = DialogController.instance.condition && res;
    }
    public void ProtectorComplete() //Нейропротектор готов
    {
        bool res = Command.HasActiveTask(QuestProtector.questName) && QuestProtector.instance.protectorComplete;
        DialogController.instance.condition = DialogController.instance.condition && res;
    }
}
