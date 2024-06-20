using System.Collections.Generic;
using UnityEngine;

public class PatrolPath : MonoBehaviour
{
    public List<Transform> path = new List<Transform>();
    public HumanAI ai;
    public void SetPath(HumanAI x)
    {
        x.RemoveState?.Invoke();
        x.patrolMap.Clear();
        foreach (var p in path)
        {
            x.patrolMap.Add(p.position);
        }
        x.patrol = true;
        ai = x;
        ai.owner.OnDeath.AddListener(RemovePath);
        ai.RemoveState.AddListener(RemovePath);
    }

    public void Start()
    {
        if (ai)
        {
            SetPath(ai);
        }
    }
    public void RemovePath()
    {
        RemovePath(null, null);
    }
    public void RemovePath(Damageable human = null, Damageable killer = null)
    {
        if (ai)
        {
            ai.RemoveState.RemoveListener(RemovePath);
            ai.owner.OnDeath.RemoveListener(RemovePath);
            ai.patrolMap.Clear();
            ai.patrol = false;
            ai = null;
        }
    }
}
