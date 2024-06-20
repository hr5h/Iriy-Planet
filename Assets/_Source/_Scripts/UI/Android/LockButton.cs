using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine.Events;

public class LockButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IDropHandler, IPointerDownHandler
{
    public UnityEvent OnButtonDown;
    public Image Image { get => _image; private set => _image = value; }
    private Image _image;
    private Color _defaultColor;
    [SerializeField] private Color _activeColor;
    public bool active;
    public bool locked;

    private void Awake()
    {
        _image = GetComponent<Image>();
        _defaultColor = _image.color;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (!locked && !active)
        {
            active = true;
            _image.color = _activeColor;
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (!locked && active)
        {
            active = false;
            _image.color = _defaultColor;
        }
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        Deactivate();
        OnButtonDown?.Invoke();
    }

    public void OnDrop(PointerEventData eventData)
    {
        locked = true;
        _image.color = _activeColor;
    }

    public void Deactivate()
    {
        gameObject.SetActive(false);
        locked = false;
        active = false;
        _image.color = _defaultColor;
    }
    public void Lock()
    {
        locked = true;
        active = true;
        _image.color = _activeColor;
    }
}
