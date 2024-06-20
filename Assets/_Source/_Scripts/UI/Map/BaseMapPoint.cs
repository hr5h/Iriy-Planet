using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BaseMapPoint : MonoBehaviour
{
    private Image _image;
    private void Awake()
    {
        _image = GetComponent<Image>();
    }
    public void SetSprite(Sprite sprite)
    {
        _image.sprite = sprite;
    }

    public void Draw(Vector2 pos, Color col, float size = 4)
    {
        _image.enabled = true;
        transform.localPosition = pos;
        _image.color = col;
        _image.rectTransform.sizeDelta = Vector2.one * size;
    }

    public void Draw(Vector2 pos, Color col, float size, Sprite sprite)
    {
        _image.enabled = true;
        transform.localPosition = pos;
        _image.color = col;
        _image.sprite = sprite;
        _image.rectTransform.sizeDelta = Vector2.one * size;
    }

    public void Clear()
    {
        _image.enabled = false;
    }
}
