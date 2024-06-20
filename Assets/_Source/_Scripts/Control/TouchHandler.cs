using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using Cinemachine;
using UnityEngine.UI;

public class TouchHandler : MonoBehaviour
{
    private bool _uiClicked;
    private readonly List<RaycastResult> _results = new List<RaycastResult>();
    private readonly PointerEventData _eventData = new PointerEventData(EventSystem.current);
    public Transform debug;
    private bool IsPointerOverUIObject(Vector3 position)
    {
        _results.Clear();
        _eventData.position = position;
        if (debug != null) 
            debug.position = position;
        EventSystem.current.RaycastAll(_eventData, _results);
        //Logger.Debug(_results.Count);
        return _results.Count > 0;
    }
    private void OnButtonPressed()
    {
        _uiClicked = true;
    }
    private void Start()
    {
        EventManager.Instance.OnButtonPressed.AddListener(OnButtonPressed);
        CinemachineCore.CameraUpdatedEvent.AddListener(OnCameraUpdated);
    }

    private void OnCameraUpdated(CinemachineBrain arg0)
    {
        if (_uiClicked) { _uiClicked = false; return; }
#if UNITY_EDITOR
        if (Input.GetMouseButtonDown(0))
        {
            if (!IsPointerOverUIObject(Input.mousePosition))
            {
                EventManager.Instance.OnTouchBegan?.Invoke(Input.mousePosition);
            }
        }
#else
        for (int i = 0; i < Input.touchCount; ++i)
        {
            var touch = Input.GetTouch(i);
            if (touch.phase == TouchPhase.Began)
            {
                if (!IsPointerOverUIObject(touch.position))
                {
                    EventManager.Instance.OnTouchBegan?.Invoke(touch.position);
                }
            }
        }
#endif
    }
}
