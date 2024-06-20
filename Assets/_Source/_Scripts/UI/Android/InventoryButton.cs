using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public class InventoryButton : ScreenButton
{
    void Start()
    {
        ButtonDown.AddListener(WorldController.Instance.playerControl.InventoryButton);
    }
}
