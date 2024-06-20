using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapLabel : MonoBehaviour
{
    protected Vector2Int _currentChunk;
    protected EntityRepository Entities => WorldController.Instance.Entities;
    protected ChunkManager Chunks => WorldController.Instance.ChunkManager;

    public float size;
    public Sprite sprite;
    public Color color;
    public bool alwaysShow; // Если флаг активен, то метка будет показываться на карте, даже если не находится в исследованной области
    public bool showOnMap;
    public bool showOnMinimap;

    public string title;
    public string description;

    private void OnEnable() => StartCoroutine(OnEnableCoroutine());
    private IEnumerator OnEnableCoroutine() //Так как OnEnable данного класса может вызваться раньше, чем Awake класса WorldController, то ожидаем один кадр
    {
        yield return null;
        EnableAction();
    }
    private void OnDisable()
    {
        DisableAction();
    }
    protected virtual void EnableAction()
    {
        Entities.Add(this);
        _currentChunk = Chunks.CalculateCurrentChunk(transform.position);
        Chunks.Add(_currentChunk, this);
    }
    protected virtual void DisableAction()
    {
        Chunks.Remove(_currentChunk, this);
        Entities.Remove(this);
    }
}
