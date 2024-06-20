using Cinemachine;
using UnityEngine;

public class Background : MonoBehaviour
{
    private float HorizontalShift = 0; //Сдвиг фона по горизонтали
    private float VerticalShift = 0; //Сдвиг фона по вертикали
    private Vector3 _prevPos;

    private Vector2 _offset;

    private SpriteRenderer _spriteRenderer;
    private Material _material;

    public Transform cam;

    public float _width;
    public float _height;

    private void OnCameraUpdated(CinemachineBrain arg0)
    {
        var pos = cam.position;
        if (pos != _prevPos)
        {
            Move(pos.x - _prevPos.x, pos.y - _prevPos.y);
            _prevPos = cam.position;
        }
    }

    private void Start()
    {
        CinemachineCore.CameraUpdatedEvent.AddListener(OnCameraUpdated);
        _spriteRenderer = GetComponent<SpriteRenderer>();
        _width = _spriteRenderer.sprite.textureRect.width * 30;
        _height = _spriteRenderer.sprite.texture.height * 30;
        _material = _spriteRenderer.sharedMaterial;
    }
    public void Move(float x, float y) //Функция сдвига фона
    {
        HorizontalShift += x / _width;
        VerticalShift += y / _height;
        _offset = new Vector2(HorizontalShift, VerticalShift);
        
        _material.SetVector(ShaderParams.offset, _offset);
    }
}
