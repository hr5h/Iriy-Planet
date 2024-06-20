using UnityEngine;

public class MechanicTraderAnswerConditions : MonoBehaviour
{
    //TODO MechanicTraderAnswerConditions Условия в диалоге с торговцем механиков
    public void NestsDestroyed() //Игрок уничтожил муравейники
    {
        bool res = Command.HasTask("Муравейники") && (QuestNestDestroy.instance.nests == null);
        DialogController.instance.condition = DialogController.instance.condition && res;
    }
    public void HasJob() //У торговца есть задание
    {
        bool res = !Command.HasTask("Муравейники");
        DialogController.instance.condition = DialogController.instance.condition && res;
    }
}
