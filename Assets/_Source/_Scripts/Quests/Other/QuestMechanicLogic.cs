using System.Collections;
using UnityEngine;

public class QuestMechanicLogic : MonoBehaviour
{
    public Human mechanic;
    public bool repair = false;
    private void Awake()
    {
        mechanic = GetComponent<Human>();
    }
    public IEnumerator RepairInterval()
    {
        yield return Yielders.Get(1);
        if (repair)
        {
            QuestShipRepair.instance.progress++;
            QuestShipRepair.instance.ProgressChanged?.Invoke();
            if (QuestShipRepair.instance.progress > QuestShipRepair.instance.maxProgress)
            {
                QuestShipRepair.instance.CompleteQuest();
            }
            else
            {
                StartCoroutine(RepairInterval());
            }
        }
    }
    public void StartRepair()
    {
        repair = true;
        RemoveEscort();
        mechanic.canSpeak = false;
        (mechanic.ai as HumanAI).dontCombat = true;
        mechanic.OnDamage.AddListener(AbortRepair);
        StartCoroutine(RepairInterval());
    }
    public void AbortRepair(Damageable character = null, Damageable damager = null)
    {
        repair = false;
        mechanic.canSpeak = true;
        (mechanic.ai as HumanAI).dontCombat = false;
        mechanic.OnDamage.RemoveListener(AbortRepair);
    }
    public void SetEscort()
    {
        (mechanic.ai as HumanAI).escort = true;
        (mechanic.ai as HumanAI).escortTarget = WorldController.Instance.playerControl.human.gameObject;
    }
    public void RemoveEscort()
    {
        (mechanic.ai as HumanAI).escort = false;
        (mechanic.ai as HumanAI).escortTarget = null;
    }
    public void EscortEnabled()
    {
        bool res = (mechanic.ai as HumanAI).escort;
        DialogController.instance.condition = DialogController.instance.condition && res;
    }
    public void EscortDisabled()
    {
        bool res = !(mechanic.ai as HumanAI).escort;
        DialogController.instance.condition = DialogController.instance.condition && res;
    }

    public void CanRepair()
    {
        bool res = QuestShipRepair.instance.repair;
        DialogController.instance.condition = DialogController.instance.condition && res;
    }
}
