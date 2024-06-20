using System.Collections.Generic;
using UnityEngine;

public class RoutePath : MonoBehaviour
{
    public List<Transform> path = new List<Transform>();
    public HumanAI ai;

    public void Start()
    {
        if (ai)
        {
            SetPath(ai);
        }
    }
    public void SetPath(HumanAI x)
    {
        x.RemoveState?.Invoke();
        x.RemoveState.AddListener(RemovePath);
        x.routeMap.Clear();
        ai = x;
        foreach (var p in path)
        {
            ai.routeMap.Enqueue(p.position);
        }
        ai.route = true;
    }
    public void RemovePath()
    {
        ai.RemoveState.RemoveListener(RemovePath);
        ai.routeMap.Clear();
        ai.route = false;
        ai = null;
    }
}
