using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public class TaskButton : ScreenButton
{
    void Start()
    {
        ButtonDown.AddListener(WorldController.Instance.playerControl.TaskButton);
    }
}
