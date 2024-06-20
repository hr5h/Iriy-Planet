using UnityEngine;

public class PilotLogic : MonoBehaviour
{
    public Human pilot;
    public bool pilotHasProtector = false;
    private void Awake()
    {
        pilot = GetComponent<Human>();
    }
    public void TakeProtector()
    {
        if (Command.HasItem(WorldController.Instance.playerControl.human.inventory, "protector"))
        {
            Command.RemoveItem(WorldController.Instance.playerControl.human.inventory, "protector");
            Command.GiveItem(pilot.inventory, "protector");
            pilotHasProtector = true;
        }
    }
    public void SetEscort()
    {
        (pilot.ai as HumanAI).escort = true;
        (pilot.ai as HumanAI).escortTarget = WorldController.Instance.playerControl.human.gameObject;
    }
    public void RemoveEscort()
    {
        (pilot.ai as HumanAI).escort = false;
        (pilot.ai as HumanAI).escortTarget = null;
    }

    public void ProtectorAnswer()
    {
        bool res = !pilotHasProtector && Command.HasItem(WorldController.Instance.playerControl.human.inventory, "protector");
        DialogController.instance.condition = DialogController.instance.condition && res;
    }
    public void EscortEnabled()
    {
        bool res = (pilot.ai as HumanAI).escort;
        DialogController.instance.condition = DialogController.instance.condition && res;
    }
    public void EscortDisabled()
    {
        bool res = !(pilot.ai as HumanAI).escort;
        DialogController.instance.condition = DialogController.instance.condition && res;
    }
}
