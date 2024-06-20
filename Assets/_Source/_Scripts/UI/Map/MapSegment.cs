using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MapSegment : MonoBehaviour
{
    [SerializeField] private Color _openColor;
    [SerializeField] private Color _closeColor;
    [SerializeField] private Color _openSelectedColor;
    [SerializeField] private Color _closeSelectedColor;

    public new Transform transform;

    public Vector2Int coord;
    private bool _selected = false;
    private bool _open = false;

    public bool IsOpen { get => _open; private set => _open = value; }

    private Image _image;

    private void Awake()
    {
        _image = GetComponent<Image>();
        transform = GetComponent<Transform>();
    }

    public void UpdateColor()
    {
        if (WorldController.Instance.chunks.ContainsKey(coord))
        {
            _open = true;
            _image.color = _openColor;
        }
        else
        {
            _open = false;
            _image.color = _closeColor;
        }
    }
}
