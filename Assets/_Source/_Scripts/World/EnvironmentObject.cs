using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.UI.CanvasScaler;

public class EnvironmentObject : MonoBehaviour
{
    private SpriteRenderer _spriteRenderer;
    public void Init(Vector3 position, float rotation, Sprite sprite)
    {
        _spriteRenderer.sprite = sprite;
        transform.position = position;
        transform.eulerAngles = new Vector3(0, 0, rotation);
    }
    public void Init(Vector3 position, float rotation, Sprite sprite, Color color)
    {
        Init(position, rotation, sprite);
        _spriteRenderer.color = color;
    }
    private void Awake()
    {
        _spriteRenderer = gameObject.GetComponent<SpriteRenderer>();
    }
}
