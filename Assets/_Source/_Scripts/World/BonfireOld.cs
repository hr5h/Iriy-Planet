using System.Collections;
using UnityEngine;

public class BonfireOld : MonoBehaviour
{
    private EntityRepository Entities => WorldController.Instance.Entities;
    private Vector2Int _currentChunk;

    public WorldController worldController;
    public bool dontDestroy;
    void Awake()
    {
        worldController = GameObject.FindGameObjectWithTag("WorldController").GetComponent<WorldController>();
    }
    public void Delete()
    {
        if (!dontDestroy)
        {
            Destroy(gameObject);
        }
    }
    private void OnEnable() => StartCoroutine(OnEnableCoroutine());
    private IEnumerator OnEnableCoroutine() //Так как OnEnable данного класса может вызваться раньше, чем Awake класса WorldController, то ожидаем один кадр
    {
        yield return null;
        _currentChunk = WorldController.Instance.ChunkManager.CalculateCurrentChunk(transform.position);
        WorldController.Instance.ChunkManager.Add(_currentChunk, this);
        Entities.Add(this);
    }
    private void OnDisable()
    {
        WorldController.Instance.ChunkManager.Remove(_currentChunk, this);
        Entities.Remove(this);
    }
}
