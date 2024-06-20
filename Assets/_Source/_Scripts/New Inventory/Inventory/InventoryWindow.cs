using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InventoryWindow : MonoBehaviour
{
    public int width;
    public int height;
    public InventoryCell.MyType cellType;
    public GameObject CellPref;
    public Inventory inventory;
    public List<InventoryCell> cells = new List<InventoryCell>();
    public RectTransform background;
    private Canvas _canvas;

    [ContextMenu("инициализаци¤")]
    public void EditorAwake()
    {
        Logger.Debug("инициализаци¤ выполнена");
        background.sizeDelta = new Vector3(100 * width + 10, 100 * height + 10);
    }
    //public void Resize()
    //{
    //    for (int i = 0; i < cells.Count; i++)
    //    {
    //        Destroy(cells[i].gameObject);
    //    }
    //    cells.Clear();
    //}

    private void Start()
    {
        _canvas = GetComponent<Canvas>();
        //Resize();
        if (inventory != null)
        {
            inventory.OnCellUpdate.AddListener(CellRefresh);
            inventory.OnItemUpdate += CellRefresh;
        }
        background.sizeDelta = new Vector3(100 * width + 10, 100 * height + 10);
        for (int i = 0; i < height; i++)
        {
            for (int j = 0; j < width; j++)
            {
                cells.Add((Instantiate(CellPref, transform)).GetComponent<InventoryCell>());
                cells[cells.Count - 1].transform.localPosition += new Vector3(j * 100 + 50 - 50 * width, -i * 100 - 50 + 50 * height);
                cells[cells.Count - 1].inventory = inventory;
                cells[cells.Count - 1].type = cellType;
                cells[cells.Count - 1].index = cells.Count - 1;
                cells[cells.Count - 1].info = InventoryController.Instance.info;
                cells[cells.Count - 1].textInfo = InventoryController.Instance.textInfo;
                if (inventory != null)
                {
                    cells[cells.Count - 1].state = inventory.items[cells.Count - 1];
                    cells[cells.Count - 1].Refresh();
                }
            }
        }
    }
    public void Open()
    {
        Refresh();
        _canvas.enabled = true;
    }
    public void Close()
    {
        _canvas.enabled = false;
    }
    //»зменить тип ¤чеек (дл¤ торговли и обмена)
    public void ChangeCellType(InventoryCell.MyType type)
    {
        for (int i = 0; i < cells.Count; i++)
        {
            cells[i].type = type;
        }
    }
    public void CellRefresh(int ind)
    {
        cells[ind].state = inventory.items[ind];
        cells[ind].Refresh();
    }

    public void ChangeInventory(Inventory inv)
    {
        inventory.OnCellUpdate.RemoveListener(CellRefresh);
        inventory.OnItemUpdate -= CellRefresh;
        inventory = inv;
        inventory.OnCellUpdate.AddListener(CellRefresh);
        inventory.OnItemUpdate += CellRefresh;
        for (int i = 0; i < cells.Count; i++)
        {
            cells[i].inventory = inventory;
            CellRefresh(i);
        }
    }
    //private void OnEnable()
    //{
    //    Refresh();
    //}
    public void Refresh()
    {
        if (inventory != null)
        {
            for (int i = 0; i < cells.Count; i++)
            {
                CellRefresh(i);
            }
        }
    }
    public void Refresh(int i)
    {
        if (inventory != null)
        {
            if (inventory.items[i] != null)
            {
                CellRefresh(i);
            }
        }
    }
    public void Clear()
    {
        inventory = null;
        for (int i = 0; i < height * width; i++)
        {
            cells[i].rend.sprite = null;
        }
    }

}
