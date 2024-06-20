using UnityEngine;

public class QuestInventory : MonoBehaviour
{
    public static QuestInventory instance;
    public static string questName;

    public void StartQuest()
    {
        Command.AddTask(questName, "Найти клавишу, с помощью которой открывается инвентарь главного героя. Крайне сложное задание, мало кому удается его выполнить");
        InventoryController.Instance.OnInventoryOpen.AddListener(CheckInventory);
    }
    public void CheckInventory()
    {
        if (InventoryController.Instance.inventory)
        {
            CompleteQuest();
        }
    }
    public void CompleteQuest()
    {
        Command.CompleteTask(questName);
        Destroy(gameObject);
    }
    private void Start()
    {
        instance = this;
        questName = "Открыть инвентарь";
        StartQuest();
    }
}
