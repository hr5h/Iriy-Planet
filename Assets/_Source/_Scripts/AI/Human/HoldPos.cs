using UnityEngine;

public class HoldPos : MonoBehaviour
{
    public HumanAI ai;
    public void SetHoldPosition(HumanAI x)
    {
        x.RemoveState?.Invoke();
        ai = x;
        ai.look = transform.rotation;
        ai.holdPosition = true;
        ai.Place = transform;
        ai.owner.OnDeath.AddListener(RemoveHoldPosition);
        ai.RemoveState.AddListener(RemoveHoldPosition);
    }
    public void Start()
    {
        if (ai)
        {
            SetHoldPosition(ai);
        }
    }
    public void RemoveHoldPosition()
    {
        RemoveHoldPosition(null, null);
    }
    public void RemoveHoldPosition(Damageable human = null, Damageable killer = null)
    {
        if (ai)
        {
            ai.RemoveState.RemoveListener(RemoveHoldPosition);
            ai.owner.OnDeath.RemoveListener(RemoveHoldPosition);
            ai.holdPosition = false;
            ai = null;
        }
    }
}
