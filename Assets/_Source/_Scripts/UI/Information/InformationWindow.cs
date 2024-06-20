using UnityEngine;
using UnityEngine.UI;

public class InformationWindow : MonoBehaviour
{
    public static InformationWindow instance;
    public ItemInfo itemInfo;
    public PlayerInfo playerInfo;

    private Canvas _playerCanvas;
    private Canvas _itemCanvas;
    private Canvas _canvas;

    [SerializeField] private ScrollRect _playerScroll;
    [SerializeField] private ScrollRect _playerRelationsScroll;
    [SerializeField] private ScrollRect _itemMainInfoScroll;
    [SerializeField] private ScrollRect _itemDescriptionScroll;

    private void Awake()
    {
        instance = this;
        _playerCanvas = playerInfo.GetComponent<Canvas>();
        _itemCanvas = itemInfo.GetComponent<Canvas>();
        _canvas = GetComponent<Canvas>();
    }
    private void ToggleCanvases(bool playerCanvas, bool itemCanvas)
    {
        _playerCanvas.enabled = playerCanvas;
        _playerScroll.enabled = playerCanvas;
        _playerRelationsScroll.enabled = playerCanvas;

        _itemCanvas.enabled = itemCanvas;
        _itemMainInfoScroll.enabled = itemCanvas;
        _itemDescriptionScroll.enabled = itemCanvas;
    }

    public void Open()
    {
        _canvas.enabled = true;
        ToggleCanvases(true, false);
    }
    public void Close()
    {
        _canvas.enabled = false;
        ToggleCanvases(false, false);
    }
    public void ShowItemInfo(InventoryCell inventoryCell)
    {
        ToggleCanvases(false, true);
        itemInfo.Show(inventoryCell);
    }

    public void HideItemInfo()
    {
        ToggleCanvases(true, false);
    }
}
