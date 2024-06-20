using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Joystick : MonoBehaviour, IPointerDownHandler, IDragHandler, IPointerUpHandler
{
    [SerializeField] private LockButton _lockButton;
    Image joystickBg;
    Image joystick;
    Vector2 inputVector;
    void Start()
    {
        joystickBg = GetComponent<Image>();
        joystick = transform.GetChild(0).GetComponent<Image>();
        _lockButton.gameObject.SetActive(false);
        _lockButton.OnButtonDown.AddListener(Unlock);
        _lockButton.transform.position = Vector2.zero;
        _lockButton.transform.localPosition = transform.position;
    }
    private void Unlock()
    {
        inputVector = Vector2.zero;
    }
    public void OnDrag(PointerEventData eventData)
    {
        if (RectTransformUtility.ScreenPointToLocalPointInRectangle(joystickBg.rectTransform, eventData.position, eventData.pressEventCamera, out Vector2 pos))
        {
            pos.x /= joystickBg.rectTransform.sizeDelta.x;
            pos.y /= joystickBg.rectTransform.sizeDelta.y;
        }
        inputVector = new Vector2(pos.x * 2, pos.y * 2);
        inputVector = (inputVector.magnitude > 1.0f) ? inputVector.normalized : inputVector;
        if (!_lockButton.locked)
        {
            joystick.rectTransform.anchoredPosition = 0.5f * inputVector * joystickBg.rectTransform.sizeDelta;

            if (inputVector.magnitude >= 0.7f)
            {
                _lockButton.gameObject.SetActive(true);
                _lockButton.Image.rectTransform.anchoredPosition = 0.8f * inputVector.normalized * joystickBg.rectTransform.sizeDelta;
            }
            else
            {
                _lockButton.gameObject.SetActive(false);
            }
        }
    }
    public void OnPointerDown(PointerEventData eventData)
    {
        _lockButton.Deactivate();
        OnDrag(eventData);
    }
    public void OnPointerUp(PointerEventData eventData)
    {
        if (!_lockButton.active)
        {
            inputVector = Vector2.zero;
            _lockButton.Deactivate();
        }
        else
        {
            inputVector = inputVector.normalized;
            _lockButton.Lock();
        }
        joystick.rectTransform.anchoredPosition = Vector2.zero;
    }
    public float Horizontal(bool keyboard = true)
    {
        if (inputVector.x != 0)
            return inputVector.x;
        else if (keyboard)
            return Input.GetAxis("Horizontal");
        return 0;
    }
    public float Vertical(bool keyboard = true)
    {
        if (inputVector.y != 0)
            return inputVector.y;
        else if (keyboard)
            return Input.GetAxis("Vertical");
        return 0;
    }
}
