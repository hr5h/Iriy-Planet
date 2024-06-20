using UnityEngine;

public class MyCursor : MonoBehaviour
{
    private Camera mainCamera;
    private Transform _transform;
    void Start()
    {
        mainCamera = Camera.main;
        Cursor.visible = false;
        _transform = GetComponent<Transform>();
    }
    private void Update()
    {
        _transform.position = mainCamera.ScreenToWorldPoint(Input.mousePosition) + new Vector3(0, 0, 10);
    }
}
