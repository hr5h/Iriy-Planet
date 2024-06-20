using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public class MapButton : ScreenButton
{
    void Start()
    {
        ButtonDown.AddListener(WorldController.Instance.playerControl.MapButton);
    }
}