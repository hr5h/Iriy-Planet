using UnityEngine;

public class ShipRepairArea : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.TryGetComponent(out Human obj))
        {
            if (QuestShipRepair.instance.mechanics.Contains(obj))
            {
                print("start repair");
                obj.GetComponent<QuestMechanicLogic>().StartRepair();
            }
        }
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.TryGetComponent(out Human obj))
        {
            if (QuestShipRepair.instance.mechanics.Contains(obj))
            {
                obj.GetComponent<QuestMechanicLogic>().AbortRepair();
            }
        }
    }
}
