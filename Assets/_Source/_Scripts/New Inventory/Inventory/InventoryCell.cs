using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class InventoryCell : MonoBehaviour, IPointerDownHandler, IDragHandler, IPointerUpHandler, IUpdatable
{
    public GameObject icon;
    public RectTransform iconRect;
    public Image rend;
    public int index;
    public Inventory inventory;
    public ItemState state;
    public TextMeshProUGUI counter;
    public GameObject info;
    public TextMeshProUGUI textInfo;
    public Vector2 icon_offset;
    public bool drag;
    private int _pointerId;

    public enum MyType
    {
        Default, // Обычный инвентарь
        TraderSlot, //Окно торговца
        CustomerSlot, //Окно покупателя
        SaleSlot, //Окно продажи
        PurchaseSlot, //Окно покупки
        WeaponSlot, //Слот для оружия
        KnifeSlot, //Слот для ножей
        ArmorSlot, //Слот для брони
        MineralSlot, //Слот для минералов
    }
    public MyType type;

    public delegate void InventoryAction(InventoryCell cell);
    public UnityEvent<InventoryCell> mouseDown;
    public UnityEvent<InventoryCell> mouseUp;
    public UnityEvent<InventoryCell> mouseEnter;
    public UnityEvent<InventoryCell> mouseExit;

    public void Clear()
    {
        drag = false;
        if (inventory && inventory.items[index] == state) { inventory.Remove(index); }
        counter.text = "";
        rend.sprite = null;
        rend.color = new Color(0, 0, 0, 0);
        state = null;
        if (select)
        {
            StartCoroutine(CoroutineInfo(false));
        }
    }
    public void OnInventoryClose()
    {
        select = false;
    }
    public void Refresh()
    {
        if (state != null && state.data != null)
        {
            rend.color = Color.white;
            rend.sprite = state.data.Icon;
            var width = state.data.Icon.bounds.size.x;
            var height = state.data.Icon.bounds.size.y;
            var lenght = Mathf.Sqrt(width * width + height * height);
            iconRect.localScale = new Vector2((88 / lenght) * width, (88 / lenght) * height);
            //rend.gameObject.transform.localScale = new Vector2(44 / lenght, 44 / lenght);

            var cs = Mathf.Cos(Mathf.Deg2Rad * 90);
            //Logger.Debug(- width * 0.5f + state.data.Icon.pivot.x);
            //Logger.Debug(- height * 0.5f + state.data.Icon.pivot.y);
            var xOld = -width * 0.5f + state.data.Icon.pivot.x;
            var yOld = -height * 0.5f + state.data.Icon.pivot.y;
            var xNew = xOld * cs - yOld * cs;
            var yNew = yOld * cs + xOld * cs;
            icon_offset = new Vector3(xNew, yNew) * 0.5f;
            iconRect.localPosition = -icon_offset;
            //rend.gameObject.transform.localPosition = icon_offset;
            //Logger.Debug(state.GetType().Name);
            if (state is CollectableItemState)
            {
                counter.text = (state as CollectableItemState).count.ToString();
            }
            else
            {
                counter.text = "";
            }
            if (select)
            {
                StartCoroutine(CoroutineInfo(true));
            }
        }
        else
        {
            Clear();
        }
    }
    bool select;
    public void Start()
    {
        drag = false;
        select = false;
        InventoryController.Instance.OnInventoryClose.AddListener(OnInventoryClose);
        mouseDown.AddListener(InventoryController.Instance.OnCellMouseDown);
        mouseUp.AddListener(InventoryController.Instance.OnCellMouseUp);
    }
    private void OnEnable() => StartCoroutine(OnEnableCoroutine());
    private IEnumerator OnEnableCoroutine()
    {
        yield return null;
        WorldController.Instance.Updater.Add(this);
    }
    private void OnDisable()
    {
        WorldController.Instance.Updater.Remove(this);
    }
    public void ManualUpdate()
    {
        if (select && Input.GetMouseButtonDown(0))
            mouseDown?.Invoke(this);
        if (select && Input.GetMouseButtonUp(0))
            mouseUp?.Invoke(this);
    }
    //private void OnMouseEnter()
    //{
    //    select = true;
    //    if (state!=null && state.data != null)
    //        StartCoroutine(CoroutineInfo(true));
    //    mouseEnter?.Invoke(this);
    //}
    //private void OnMouseExit()
    //{
    //    select = false;
    //    if (state != null && state.data != null)
    //        StartCoroutine(CoroutineInfo(false));
    //    mouseExit?.Invoke(this);
    //}
    private void Info(bool input)
    {
        if (input)
        {
            info.SetActive(true);
            info.transform.position = new Vector3(this.transform.position.x - 110, this.transform.position.y - 110, this.transform.position.z);
            switch (state)
            {
                case WeaponState state:
                    {
                        textInfo.text = "<b>" + state.data.Title + "</b>\n\n" + "<b>Урон:</b> " +
                            ((WeaponData)state.data).damage + "\n\n" + "<b>Износ:</b> " + state.condition + "/" + ((WeaponData)state.data).conditionMax +
                            "\n\n" + "<b>Тип патронов:</b> " + ((WeaponData)state.data).ammoType + "\n\n" +
                            "<b>Размер обоймы:</b> " + ((WeaponData)state.data).clip + "\n\n" + "<b>Длительность перезарядки:</b> " +
                            ((WeaponData)state.data).reloadTime + "\n\n" + "<b>Дальность стрельбы:</b> " + ((WeaponData)state.data).range + "\n\n" +
                            "<b>Скорость пули:</b> " + ((WeaponData)state.data).bulletSpeed + "\n\n" + "<b>Снижение скорости передвижения:</b> " +
                            ((WeaponData)state.data).movementCoef + "\n\n<i>" + ((WeaponData)state.data).PrintHelp() + "</i>\n\n"; break;
                    }
                case MineralState state: { textInfo.text = "<b>" + state.data.Title + "</b>\n\n" + ((MineralData)state.data).PrintHelp(); break; }
                case ArmorState state:
                    {
                        textInfo.text = "<b>" + state.data.Title + "</b>\n\n" + "<b>Бонус к броне:</b> " + ((ArmorData)state.data).armorBonus + "\n\n" +
                            "<b>Бонус к передвижению:</b> " + ((ArmorData)state.data).movementBonus + "\n\n" + "<b>Износ:</b> " + state.condition + "/" +
                            ((ArmorData)state.data).conditionMax + "\n\n"; break;
                    }
                case KnifeState state:
                    {
                        textInfo.text = "<b>" + state.data.Title + "</b>\n\n" + "<b>Урон:</b> " + ((KnifeData)state.data).damage + "\n\n" +
                            "<b>Интервал между ударами:</b> " + ((KnifeData)state.data).rate + "\n\n"; break;
                    }
                case MedicineState state:
                    {
                        textInfo.text = "<b>" + state.data.Title + "</b>\n\n" + "<b>Восстановление здоровья:</b> " + ((MedicineData)state.data).heal + "\n\n"; break;
                    }
                default: { textInfo.text = "<b>" + state.data.Title + "</b>\n\n"; break; }
            }
            textInfo.text += "<b>Описание:</b> " + state.data.Description + "\n\n";
            if (state.data.Cost != 0)
                textInfo.text += "<b>Цена:</b> " + state.data.Cost;
            else
                textInfo.text += "<color=red>Нельзя продать</color> ";
        }
        else
        {
            textInfo.text = "";
            info.SetActive(false);
        }
    }
    private IEnumerator CoroutineInfo(bool input)
    {
        yield return Yielders.Get(0.1f);
        Info(input);
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (InventoryController.Instance.first == null && state != null && state.data != null)
        {
            _pointerId = eventData.pointerId;
            InventoryController.Instance.dragElement.gameObject.SetActive(true);
            InventoryController.Instance.dragElement.rend.sprite = rend.sprite;
            InventoryController.Instance.dragElement.transform.localScale = rend.transform.localScale;
            InventoryController.Instance.dragElement.transform.localPosition = -icon_offset;
            InventoryController.Instance.dragElement.transform.position = eventData.position;

            rend.gameObject.SetActive(false);
            drag = true;
            InformationWindow.instance.ShowItemInfo(this);
            mouseDown?.Invoke(this);
        }
        else
        {
            InformationWindow.instance.HideItemInfo();
        }
    }
    public void OnDrag(PointerEventData eventData)
    {
        if (_pointerId != eventData.pointerId)
            return;
        if (drag)
        {
            InventoryController.Instance.dragElement.transform.position = eventData.position;
        }
        else
        {
            InventoryController.Instance.dragElement.gameObject.SetActive(false);
            rend.gameObject.SetActive(true);
            eventData.pointerDrag = null;
        }
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        if (_pointerId != eventData.pointerId)
            return;
        if (drag)
        {
            InventoryController.Instance.dragElement.gameObject.SetActive(false);

            drag = false;
            rend.gameObject.SetActive(true);
            List<RaycastResult> results = new List<RaycastResult>();
            EventSystem.current.RaycastAll(eventData, results);
            foreach (var result in results)
            {
                if (result.gameObject.TryGetComponent(out InventoryCell cell))
                {
                    if (result.gameObject != gameObject)
                    {
                        mouseUp?.Invoke(cell);
                    }
                    return;
                }
            }
        }
    }
}
