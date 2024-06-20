using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public class JournalButton : ScreenButton
{
    void Start()
    {
        ButtonDown.AddListener(WorldController.Instance.playerControl.JournalButton);
    }
}
