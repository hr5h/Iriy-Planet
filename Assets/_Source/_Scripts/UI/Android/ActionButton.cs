using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ActionButton : ScreenButton
{
    private Image image;
    void Start()
    {
        image = GetComponent<Image>();
        image.enabled = false;
        ButtonDown.AddListener(WorldController.Instance.playerControl.ActionButton);
        WorldController.Instance.playerControl.shopOpener.OnCanInterract.AddListener(ShowButton);
        WorldController.Instance.playerControl.shopOpener.OnDontCanInterract.AddListener(HideButton);
    }
    void ShowButton()
    {
        image.enabled = true;
    }
    void HideButton()
    {
        image.enabled = false;
    }
}
