using System.Collections;
using UnityEngine;

public class DropItems : MonoBehaviour
{
    private EntityRepository Entities => WorldController.Instance.Entities;
    private Vector2Int _currentChunk;

    public GameObject dropButton;
    [HideInInspector]
    public bool button = false;
    public Inventory inventory;

    public bool toDestroy;
    public bool dontDestroy;
    public bool firstTime = true;
    public int money;
    public bool isStash;

    public bool IsQuestItems()
    {
        for (int i = 0; i < inventory.items.Count; i++)
        {
            if (inventory.items[i] != null && inventory.items[i].data.questItem)
                return true;
        }
        return false;
    }
    private void OnEnable() => StartCoroutine(OnEnableCoroutine());
    private IEnumerator OnEnableCoroutine() //Так как OnEnable данного класса может вызваться раньше, чем Awake класса WorldController, то ожидаем один кадр
    {
        yield return null;
        Entities.Add(this);
        _currentChunk = WorldController.Instance.ChunkManager.CalculateCurrentChunk(transform.position);
        WorldController.Instance.ChunkManager.Add(_currentChunk, this);
    }
    private void OnDisable()
    {
        WorldController.Instance.ChunkManager.Remove(_currentChunk, this);
        Entities.Remove(this);
    }
}
