using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class FlashlightButton : ScreenButton
{
    private Image image;
    private Color defaultColor;
    [SerializeField] private Color activeColor;
    public override void OnPointerDown(PointerEventData eventData)
    {
        base.OnPointerDown(eventData);
        CheckTorchState();
    }

    void Start()
    {
        image = GetComponent<Image>();
        defaultColor = image.color;
        CheckTorchState();
        ButtonDown.AddListener(WorldController.Instance.playerControl.FlashlightButton);
    }

    private void CheckTorchState()
    {
        if (WorldController.Instance.playerControl.torchLight.activeSelf)
            image.color = activeColor;
        else
            image.color = defaultColor;
    }
}
