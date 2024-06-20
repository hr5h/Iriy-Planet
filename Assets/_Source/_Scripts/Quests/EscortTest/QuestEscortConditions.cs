using UnityEngine;

public class QuestEscortConditions : MonoBehaviour
{
    public void EscortEnabled() //Бот сопровождает игрока
    {
        DialogController.instance.condition = DialogController.instance.condition && QuestEscort.instance.bot.escort;
    }
    public void EscortDisabled() //Бот не сопровождает игрока
    {
        DialogController.instance.condition = DialogController.instance.condition && !QuestEscort.instance.bot.escort;
    }
}
