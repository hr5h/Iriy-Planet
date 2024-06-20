using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public class ScreenButton : MonoBehaviour, IPointerDownHandler
{
    public UnityEvent ButtonDown;
    public virtual void OnPointerDown(PointerEventData eventData)
    {
        EventManager.Instance.OnButtonPressed?.Invoke();
        ButtonDown?.Invoke();
    }
}
