using Game.Sounds;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class InventoryController : MonoBehaviour
{
    public static InventoryController Instance { get; private set; }

    //Окошко информации
    public GameObject info;
    public TextMeshProUGUI textInfo;

    public InformationWindow informationWindow;

    public Inventory PlayerInventory;
    public Inventory OtherInventory;
    public Inventory PurchaseInventory;
    public Inventory SaleInventory;

    public InventoryWindow InventoryUI;
    public InventoryWindow PlayerInventoryUI;
    public InventoryWindow OtherInventoryUI;
    public InventoryWindow PurchaseInventoryUI;
    public InventoryWindow SaleInventoryUI;
    public CheckUI checkUI;
    public DragElement dragElement;

    public InventoryCell WeaponSlot;
    public InventoryCell ArmorSlot;
    public InventoryCell KnifeSlot;
    public List<InventoryCell> MineralSlot = new List<InventoryCell>(4);
    [HideInInspector] public UnityEvent OnInventoryClose; ///Событие закрытия инвентаря
    [HideInInspector] public UnityEvent OnInventoryOpen; ///Событие открытия инвентаря

    ///Торговля
    public TradingUI tradingUI;
    public float tradeCoef; //Коэфициент торговли для игрока. Если равен 1, то покупка и продажа по одной цене. Если < 1, то покупка дороже продажи.
    public int salePrice; //Стоимость продажи предметов игроком
    public int purchasePrice; //Стоимость покупки предметов игроком

    [HideInInspector] public UnityEvent OnChangedSalePrice;
    [HideInInspector] public UnityEvent OnChangedPurchasePrice;


    public bool trading;
    public bool exchange;
    public bool inventory;

    public TextMeshProUGUI playerMoney;
    public Human player; ///Игрок

    public InventoryCell first;
    public InventoryCell second;
    public InventoryCell double_click;
    public Camera mainCamera;
    public float time;

    //AudioSource
    public AudioSource audioSource;
    //AudioClip
    public AudioClip useMedicineClip;
    public AudioClip WeaponClip;
    public AudioClip ArmorClip;
    public AudioClip MineralClip;
    public AudioClip openInvClip;
    public AudioClip closeInvClip;

    public PlayerControl playerController;

    //Открытие окон инвентаря
    public void InventoryOpen()
    {
        playerController.showDrop = true;
        AudioPlayer.Instance.PlayUI(openInvClip);
        Command.CloseAllWindow();
        InventoryUI.Open();
        informationWindow.Open();
        inventory = true;
        OnInventoryOpen?.Invoke();
    }
    public void TradingOpen(Inventory inv)
    {
        playerController.showDrop = true;
        if (inv != null)
        {
            Command.CloseAllWindow();

            purchasePrice = 0;
            salePrice = 0;

            OtherInventory = inv;
            OtherInventoryUI.inventory = OtherInventory;
            tradingUI.gameObject.SetActive(true);
            tradingUI.ChangeOther(OtherInventory.owner);
            tradingUI.RefreshText();
            PlayerInventoryUI.Open();
            OtherInventoryUI.Open();
            PurchaseInventoryUI.Open();
            SaleInventoryUI.Open();
            OtherInventoryUI.ChangeInventory(inv);

            PlayerInventoryUI.ChangeCellType(InventoryCell.MyType.CustomerSlot);
            OtherInventoryUI.ChangeCellType(InventoryCell.MyType.TraderSlot);
            trading = true;
            OnInventoryOpen?.Invoke();
        }
    }
    public void ExchangeOpen(Inventory inv)
    {
        playerController.showDrop = true;
        if (inv != null)
        {
            Command.CloseAllWindow();

            OtherInventory = inv;
            OtherInventoryUI.inventory = OtherInventory;
            PlayerInventoryUI.Open();
            OtherInventoryUI.Open();
            OtherInventoryUI.ChangeInventory(inv);
            checkUI.gameObject.SetActive(true);
            checkUI.ChangeText(OtherInventory.deadName);

            PlayerInventoryUI.ChangeCellType(InventoryCell.MyType.Default);
            OtherInventoryUI.ChangeCellType(InventoryCell.MyType.Default);
            exchange = true;
            OnInventoryOpen?.Invoke();
        }
    }

    //Закрытие окон инвентаря
    public void InventoryClose()
    {
        playerController.showDrop = false;
        AudioPlayer.Instance.PlayUI(closeInvClip);
        InventoryUI.Close();
        informationWindow.Close();
        OnInventoryClose?.Invoke();
        inventory = false;
        //info.SetActive(false);
    }
    public void TradingClose()
    {
        playerController.showDrop = false;
        //TODO если в инвентаре игрока или торговца вдруг не останется место, то скрипт не сможет вернуть предметы
        for (int i = 0; i < SaleInventory.items.Count; i++)
        {
            if (SaleInventory.items[i] != null)
            {
                if (SaleInventory.items[i].data is AmmoData)
                {
                    PlayerInventory.owner.TakeAmmo((SaleInventory.items[i].data as AmmoData).ammoType, (SaleInventory.items[i] as AmmoState).count);
                }
                PlayerInventory.Add(SaleInventory.items[i]);
                SaleInventory.Remove(i);
                SaleInventory.OnCellUpdate?.Invoke(i);
            }
        }
        for (int i = 0; i < PurchaseInventory.items.Count; i++)
        {
            if (PurchaseInventory.items[i] != null)
            {
                if (PurchaseInventory.items[i].data is AmmoData)
                {
                    OtherInventory.owner.TakeAmmo((PurchaseInventory.items[i].data as AmmoData).ammoType, (PurchaseInventory.items[i] as AmmoState).count);
                }
                OtherInventory.Add(PurchaseInventory.items[i]);
                PurchaseInventory.Remove(i);
                PurchaseInventory.OnCellUpdate?.Invoke(i);
            }
        }
        tradingUI.gameObject.SetActive(false);
        PlayerInventoryUI.Close();
        OtherInventoryUI.Close();
        PurchaseInventoryUI.Close();
        SaleInventoryUI.Close();
        OtherInventory = null;
        OnInventoryClose?.Invoke();
        trading = false;
        //info.SetActive(false);
    }
    public void ExchangeClose()
    {
        playerController.showDrop = false;
        PlayerInventoryUI.Close();
        OtherInventoryUI.Close();
        checkUI.gameObject.SetActive(false);
        OtherInventory = null;
        OnInventoryClose?.Invoke();
        exchange = false;
        //info.SetActive(false);
    }
    private void RefreshMoney()
    {
        playerMoney.text = "Деньги: " + player.money;
    }


    //ТОРГОВЛЯ
    //Есть ли место в инвентаре на совершение сделки?
    private bool FreeSpace()
    {
        //TODO проверить
        int it = 0;
        bool b = true;
        int count;
        for (int i = 0; i < PurchaseInventory.items.Count; i++)
        {
            if (PurchaseInventory.items[i] != null)
            {
                b = false;
                if (PurchaseInventory.items[i] is CollectableItemState)
                {
                    count = (PurchaseInventory.items[i] as CollectableItemState).count;
                    while (it < PlayerInventory.items.Count)
                    {
                        if (PlayerInventory.items[it] == null)
                        {
                            count -= (PurchaseInventory.items[i].data as CollectableItemData).countMax;
                        }
                        else
                        {
                            if (PlayerInventory.items[it].GetType() == PurchaseInventory.items[i].GetType())
                            {
                                count -= (PlayerInventory.items[it].data as CollectableItemData).countMax - (PlayerInventory.items[it] as CollectableItemState).count;
                            }
                        }
                        ++it;
                        if (count <= 0)
                        {
                            b = true;
                            break;
                        }
                    }
                }
                else
                {
                    while (it < PlayerInventory.items.Count)
                    {
                        if (PlayerInventory.items[it] == null)
                        {
                            b = true;
                            break;
                        }
                        ++it;
                    }
                }
            }
        }
        if (!b)
            return false;
        it = 0;
        b = true;
        for (int i = 0; i < SaleInventory.items.Count; i++)
        {
            if (SaleInventory.items[i] != null)
            {
                b = false;
                if (SaleInventory.items[i] is CollectableItemState)
                {
                    count = (SaleInventory.items[i] as CollectableItemState).count;
                    while (it < OtherInventory.items.Count)
                    {
                        if (OtherInventory.items[it] == null)
                        {
                            count -= (SaleInventory.items[i].data as CollectableItemData).countMax;
                        }
                        else
                        {
                            if (OtherInventory.items[it].GetType() == SaleInventory.items[i].GetType())
                            {
                                count -= (OtherInventory.items[it].data as CollectableItemData).countMax - (OtherInventory.items[it] as CollectableItemState).count;
                            }
                        }
                        ++it;
                        if (count <= 0)
                        {
                            b = true;
                            break;
                        }
                    }
                }
                else
                {
                    while (it < OtherInventory.items.Count)
                    {
                        if (OtherInventory.items[it] == null)
                        {
                            b = true;
                            break;
                        }
                        ++it;
                    }
                }
            }
        }
        return b;
    }
    //Торговля
    public void Trading()
    {
        if ((player.money + salePrice >= purchasePrice) && (OtherInventory.owner.money + purchasePrice >= salePrice) && (FreeSpace()))
        {
            for (int i = 0; i < PurchaseInventory.items.Count; i++)
            {
                if (PurchaseInventory.items[i] is AmmoState)
                {
                    var t = PurchaseInventory.items[i] as AmmoState;
                    player.TakeAmmo((t.data as AmmoData).ammoType, t.count);
                }
                PlayerInventory.Add(PurchaseInventory.items[i]);
                PurchaseInventory.items[i] = null;
                PurchaseInventory.OnCellUpdate?.Invoke(i);
            }
            for (int i = 0; i < SaleInventory.items.Count; i++)
            {
                if (SaleInventory.items[i] is AmmoState)
                {
                    var t = SaleInventory.items[i] as AmmoState;
                    OtherInventory.owner.TakeAmmo((t.data as AmmoData).ammoType, t.count);
                }
                OtherInventory.Add(SaleInventory.items[i]);
                SaleInventory.items[i] = null;
                SaleInventory.OnCellUpdate?.Invoke(i);
            }
            player.TakeMoney(salePrice - purchasePrice);
            OtherInventory.owner.TakeMoney(purchasePrice - salePrice);
            RecalculatePurchase();
            RecalculateSale();
        }
    }
    private void Awake()
    {
        PlayerInventory.owner.OnChangedMoney.AddListener(RefreshMoney);
        RefreshMoney();
        Instance = this;
        player.OnWeaponEquip.AddListener(OnPLayerWeaponEquip);
        player.OnWeaponUnEquip.AddListener(OnPLayerWeaponUnEquip);
        player.OnArmorEquip.AddListener(OnPLayerArmorEquip);
        player.OnKnifeEquip.AddListener(OnPLayerKnifeEquip);
        player.OnMineralEquip.AddListener(OnPLayerMineralEquip);
        player.OnArmorDamaged.AddListener(ArmorUpdate);
        player.OnShot.AddListener(WeaponUpdate);
    }
    public void ArmorUpdate()
    {
        (ArmorSlot.state as ArmorState).condition = player.armorCondition;
    }
    public void WeaponUpdate()
    {
        (WeaponSlot.state as WeaponState).condition = player.weapon.condition;
    }
    //Перерасчет стоимости продажи
    public void RecalculateSale()
    {
        //Logger.Debug("Recalculate");
        salePrice = 0;
        for (int i = 0; i < SaleInventory.width * SaleInventory.height; i++)
        {
            if (SaleInventory.items[i] != null)
            {
                int price;
                if (SaleInventory.items[i] is AmmoState)
                {
                    var item = (SaleInventory.items[i] as CollectableItemState);
                    price = item.data.Cost * item.count / (item.data as CollectableItemData).countMax;
                }
                else
                {
                    if (SaleInventory.items[i] is CollectableItemState)
                    {
                        var item = (SaleInventory.items[i] as CollectableItemState);
                        price = item.data.Cost * item.count;
                    }
                    else
                    {
                        switch (SaleInventory.items[i])
                        {
                            case ArmorState armor:
                                {
                                    price = Mathf.CeilToInt(SaleInventory.items[i].data.Cost * armor.condition / (armor.data as ArmorData).conditionMax);
                                }
                                break;
                            case WeaponState weapon:
                                {
                                    price = Mathf.CeilToInt(SaleInventory.items[i].data.Cost * weapon.condition / (weapon.data as WeaponData).conditionMax);
                                }
                                break;
                            default:
                                {
                                    price = SaleInventory.items[i].data.Cost;
                                }
                                break;
                        }
                    }
                }
                salePrice += Mathf.CeilToInt(price * tradeCoef);
            }
        }
        OnChangedSalePrice?.Invoke();
    }
    //Перерасчет стоимости покупки
    public void RecalculatePurchase()
    {
        purchasePrice = 0;
        for (int i = 0; i < PurchaseInventory.width * PurchaseInventory.height; i++)
        {
            if (PurchaseInventory.items[i] != null)
            {
                int price;
                if (PurchaseInventory.items[i] is AmmoState)
                {
                    var item = (PurchaseInventory.items[i] as CollectableItemState);
                    price = item.data.Cost * item.count / (item.data as CollectableItemData).countMax;
                }
                else
                {
                    if (PurchaseInventory.items[i] is CollectableItemState)
                    {
                        var item = (PurchaseInventory.items[i] as CollectableItemState);
                        price = item.data.Cost * item.count;
                    }
                    else
                    {
                        switch (PurchaseInventory.items[i])
                        {
                            case ArmorState armor:
                                {
                                    price = Mathf.CeilToInt(PurchaseInventory.items[i].data.Cost * armor.condition / (armor.data as ArmorData).conditionMax);
                                }
                                break;
                            case WeaponState weapon:
                                {
                                    price = Mathf.CeilToInt(PurchaseInventory.items[i].data.Cost * weapon.condition / (weapon.data as WeaponData).conditionMax);
                                }
                                break;
                            default:
                                {
                                    price = PurchaseInventory.items[i].data.Cost;
                                }
                                break;
                        }
                    }
                }
                purchasePrice += price;// + Mathf.FloorToInt(price * (1-tradeCoef));
            }
        }
        OnChangedPurchasePrice?.Invoke();
    }

    //Перемещает предмет из одного инвентаря в другой
    public void Push(Inventory to, InventoryCell cell)
    {
        if (cell.state is CollectableItemState)
        {
            if (cell.state is AmmoState)
            {
                var t = cell.state as AmmoState;
                int count = t.count;
                if (to.Add(t, (t.data as AmmoData).ammoCount))
                {
                    if (t.count == 0)
                        cell.inventory.Remove(cell.index);
                    cell.inventory.OnCellUpdate?.Invoke(cell.index);
                }
                if (cell.inventory.owner != null)
                {
                    cell.inventory.owner.TakeAmmo((t.data as AmmoData).ammoType, -(count - t.count));
                }
                if (to.owner != null)
                {
                    to.owner.TakeAmmo((t.data as AmmoData).ammoType, count - t.count);
                }
            }
            else
            {
                if (to.Add(cell.state as CollectableItemState, 1))
                {
                    if ((cell.state as CollectableItemState).count == 0)
                        cell.inventory.Remove(cell.index);
                    cell.inventory.OnCellUpdate?.Invoke(cell.index);
                }
            }
        }
        else
        {
            if (to.Add(cell.state))
            {
                cell.inventory.Remove(cell.index);
                cell.inventory.OnCellUpdate?.Invoke(cell.index);
            }
        }
    }
    //Меняет содержимое двух ячеек (возможно, из разных инвентарей)
    public void Swap(InventoryCell from, InventoryCell to)
    {
        if ((from.state != null) && (from.state.data is AmmoData))
        {
            if (from.inventory.owner != null)
            {
                from.inventory.owner.TakeAmmo((from.state.data as AmmoData).ammoType, -(from.state as CollectableItemState).count);
            }
            if (to.inventory.owner != null)
            {
                to.inventory.owner.TakeAmmo((from.state.data as AmmoData).ammoType, (from.state as CollectableItemState).count);
            }
        }
        if ((to.state != null) && (to.state.data is AmmoData))
        {
            if (to.inventory.owner != null)
            {
                to.inventory.owner.TakeAmmo((to.state.data as AmmoData).ammoType, -(to.state as CollectableItemState).count);
            }
            if (from.inventory.owner != null)
            {
                from.inventory.owner.TakeAmmo((to.state.data as AmmoData).ammoType, (to.state as CollectableItemState).count);
            }
        }
        if ((from.state is CollectableItemState) && (to.state is CollectableItemState) && (from.state.data == to.state.data)) //Если имеем дело с одинаковыми количественными предметами
        {
            var item1 = from.state as CollectableItemState;
            var item2 = to.state as CollectableItemState;
            int countMax = (item1.data as CollectableItemData).countMax;
            if (item2.count == countMax)
            {
                (from.inventory.items[from.index], to.inventory.items[to.index]) = (to.inventory.items[to.index], from.inventory.items[from.index]);
            }
            else
            {
                (item1.count, item2.count) = (Mathf.Max(0, item1.count + item2.count - countMax), Mathf.Min(countMax, item2.count + item1.count));
                if (item1.count == 0)
                    from.inventory.Remove(from.index);
            }
        }
        else
        {
            (from.inventory.items[from.index], to.inventory.items[to.index]) = (to.inventory.items[to.index], from.inventory.items[from.index]);
        }
        from.inventory.OnCellUpdate?.Invoke(from.index);
        to.inventory.OnCellUpdate?.Invoke(to.index);
    }
    public void OnPLayerWeaponEquip(Weapon weapon)
    {
        if (player.mainHero)
        {
            AudioPlayer.Instance.PlayUI(WeaponClip);
        }
        var state = new WeaponState
        {
            data = weapon.data,
            condition = weapon.condition
        };
        WeaponSlot.state = state;
        WeaponSlot.Refresh();

    }
    public void OnPLayerWeaponUnEquip()
    {
        if (player.mainHero)
        {
            AudioPlayer.Instance.PlayUI(WeaponClip);
        }
        WeaponSlot.Clear();
    }
    public void OnPLayerArmorEquip()
    {
        if (player.mainHero)
        {
            AudioPlayer.Instance.PlayUI(ArmorClip);
        }
        var state = new ArmorState
        {
            data = player.armorData,
            condition = player.armorCondition
        };
        ArmorSlot.state = state;
        ArmorSlot.Refresh();

    }
    public void OnPLayerMineralEquip()
    {
        if (player.mainHero)
        {
            AudioPlayer.Instance.PlayUI(MineralClip);
        }
        for (int i = 0; i < 4; i++)
        {
            if (player.minerals[i] != null)
            {
                var state = new MineralState
                {
                    data = player.minerals[i]
                };
                MineralSlot[i].state = state;
                MineralSlot[i].Refresh();
            }
        }
    }
    public void OnPLayerKnifeEquip(Knife knife)
    {
        var state = new Knife();
        //TODO
        //KnifeSlot.state = state;
        KnifeSlot.Refresh();

    }
    void UseMedicine(InventoryCell cell)
    {
        if (player.mainHero)
        {
            AudioPlayer.Instance.PlayUI(useMedicineClip);
        }
        var med = cell.state as MedicineState;
        player.TakeHeal((med.data as MedicineData).heal);
        med.count -= 1;
        if (med.count == 0)
        {
            cell.Clear();
        }
        else
        {
            cell.Refresh();
        }
    }
    void EquipWeapon(InventoryCell cell)
    {
        var weapon = Instantiate(Prefabs.weapon, player.transform);
        weapon.Init(cell.state as WeaponState);
        cell.Clear();
        if (player.weapon != null)
        {
            player.weapon.UnEquip().PickUp(player);
        }
        weapon.Equip(player);
    }
    void EquipKnife(InventoryCell cell)
    {
        //TODO EquipKnife
    }
    void EquipMineral(InventoryCell cell)
    {
        int slot = -1;
        for (int i = 0; i < 4; i++)
        {
            if (player.minerals[i] == null)
            {
                slot = i;
                break;
            }
        }
        if (slot == -1)
            return;
        player.EquipMineral(cell.state.data as MineralData, slot);
        cell.Clear();
    }
    void EquipArmor(InventoryCell cell)
    {
        var oldArmor = player.UnEquipArmor();
        player.EquipArmor(cell.state.data as ArmorData, (cell.state as ArmorState).condition);
        cell.Clear();
        PlayerInventory.Add(oldArmor);
    }
    public void OnCellMouseDown(InventoryCell cell)
    {
        if (cell.state != null)
        {
            if ((time == 0) || (double_click != cell))
            {
                first = cell;
                double_click = cell;
                time = 0.5f;
            }
            else
            {
                time = 0.5f;

                //
                cell.drag = false;
                cell.rend.gameObject.SetActive(true);
                dragElement.gameObject.SetActive(false);
                //

                switch (cell.type)
                {
                    case InventoryCell.MyType.Default:
                        {
                            //Если мы работаем только с инвентарем игрока
                            if (OtherInventory == null)
                            {
                                //Проверяем, что лежит в ячейке и предпринимаем нужное действие
                                switch (cell.state)
                                {
                                    case WeaponState _: { EquipWeapon(cell); break; }
                                    case MineralState _: { EquipMineral(cell); break; }
                                    case ArmorState _: { EquipArmor(cell); break; }
                                    case KnifeState _: { EquipKnife(cell); break; }
                                    case MedicineState _: { UseMedicine(cell); break; }
                                }
                            }
                            else
                            {
                                //Передаем предмет в другой инвентарь
                                if (cell.inventory == PlayerInventory)
                                {
                                    if (!cell.state.data.questItem)
                                        Push(OtherInventory, cell);
                                }
                                else
                                {
                                    Push(PlayerInventory, cell);
                                }
                            }
                            break;
                        }
                    //Если двойной клик по ячейке для оружия - убрать оружие со сцены и положить в инвентарь
                    case InventoryCell.MyType.WeaponSlot:
                        {
                            if (player.weapon != null)
                            {
                                player.weapon.UnEquip().PickUp(player);
                                cell.Clear();
                            }
                            break;
                        }
                    //Если двойной клик по ячейке для брони - отнять у игрока бонус к защите и положить броню в инвентарь
                    case InventoryCell.MyType.ArmorSlot:
                        {
                            (cell.state as ArmorState).condition = player.armorCondition;
                            if (PlayerInventory.Add(cell.state))
                            {
                                if (player.mainHero)
                                {
                                    AudioPlayer.Instance.PlayUI(ArmorClip);
                                }
                                player.UnEquipArmor();
                                cell.Clear();
                            }
                            break;
                        }
                    //Если двойной клик по ячейке для ножа - убрать его со сцены и положить в инвентарь
                    case InventoryCell.MyType.KnifeSlot:
                        {
                            if (PlayerInventory.Add(cell.state))
                            {
                                cell.Clear();
                            }
                            break;
                        }
                    //Если двойной клик по ячейке для минерала - отнять его свойства у игрока и вернуть в инвентарь
                    case InventoryCell.MyType.MineralSlot:
                        {
                            if (PlayerInventory.Add(cell.state))
                            {
                                if (player.mainHero)
                                {
                                    AudioPlayer.Instance.PlayUI(MineralClip);
                                }
                                player.UnEquipMineral(cell.index);
                                cell.Clear();
                            }
                            break;
                        }
                    //Если двойной клик по ячейке покупателя(игрока) - перенести предмет в ячейку для продажи
                    case InventoryCell.MyType.CustomerSlot:
                        {
                            //Проверка, что не пытаемся продать непродаваемый предмет
                            if (cell.state.data.Cost == 0 || cell.state.data.questItem)
                                return;
                            Push(SaleInventory, cell);
                            RecalculateSale();
                            break;
                        }
                    //Если двойной клик по ячейке торговца - перенести предмет в ячейку покупки
                    case InventoryCell.MyType.TraderSlot:
                        {
                            //Проверка, что не пытаемся продать непродаваемый предмет
                            if (cell.state.data.Cost == 0)
                                return;
                            Push(PurchaseInventory, cell);
                            RecalculatePurchase();
                            break;
                        }
                    //Если двойной клик по ячейке для покупки - вернуть предмет торговцу
                    case InventoryCell.MyType.PurchaseSlot:
                        {
                            Push(OtherInventory, cell);
                            RecalculatePurchase();
                            break;
                        }
                    //Если двойной клик по ячейке для продажи - вернуть предмет игроку
                    case InventoryCell.MyType.SaleSlot:
                        {
                            Push(PlayerInventory, cell);
                            RecalculateSale();
                            break;
                        }
                }
                first = null;
                second = null;
            }
        }
    }

    public void OnCellMouseUp(InventoryCell cell)
    {
        //Logger.Debug(cell.index);
        if (first != null)
        {
            second = cell;
            if (first != second)
            {
                //Перемещение в слот для минералов
                if (first.type == InventoryCell.MyType.MineralSlot)
                {
                    if (second.type == InventoryCell.MyType.MineralSlot)
                    {
                        (player.minerals[first.index], player.minerals[second.index]) = (player.minerals[second.index], player.minerals[first.index]);
                        (first.state, second.state) = (second.state, first.state);
                        first.Refresh();
                        second.Refresh();
                    }
                    else
                    {
                        if (second.state == null)
                        {
                            player.UnEquipMineral(first.index);
                            PlayerInventory.items[second.index] = first.state;
                            second.inventory.OnCellUpdate?.Invoke(second.index);
                            first.Clear();
                        }
                        else
                        {
                            if (second.state is MineralState)
                            {
                                player.UnEquipMineral(first.index);
                                PlayerInventory.items[second.index] = first.state;
                                player.EquipMineral(second.state.data as MineralData, first.index);
                                second.inventory.OnCellUpdate?.Invoke(second.index);
                            }
                        }
                    }
                    return;
                }
                if (second.type == InventoryCell.MyType.MineralSlot)
                {
                    if (first.type == InventoryCell.MyType.MineralSlot)
                    {
                        (player.minerals[first.index], player.minerals[second.index]) = (player.minerals[second.index], player.minerals[first.index]);
                        (first.state, second.state) = (second.state, first.state);
                        first.Refresh();
                        second.Refresh();
                    }
                    else
                    {
                        if (first.state is MineralState)
                        {
                            if (second.state == null)
                            {
                                player.EquipMineral(first.state.data as MineralData, second.index);
                                first.Clear();
                            }
                            else
                            {
                                player.UnEquipMineral(second.index);
                                PlayerInventory.items[first.index] = second.state;
                                player.EquipMineral(first.state.data as MineralData, second.index);
                                first.inventory.OnCellUpdate?.Invoke(first.index);
                            }
                        }
                    }
                    return;
                }
                //Перемещение в слот для оружия
                if (first.type == InventoryCell.MyType.WeaponSlot)
                {
                    if (second.state is WeaponState)
                    {
                        var weapon = player.weapon.UnEquip();
                        var state = new WeaponState
                        {
                            data = weapon.data,
                            ammo = weapon.ammo,
                            condition = weapon.condition
                        };

                        Destroy(weapon.gameObject);
                        first.Clear();
                        EquipWeapon(second);
                        PlayerInventory.items[second.index] = state;
                        second.inventory.OnCellUpdate?.Invoke(second.index);
                    }
                    else
                    {
                        if (second.state == null)
                        {
                            var weapon = player.weapon.UnEquip();
                            var state = new WeaponState
                            {
                                data = weapon.data,
                                ammo = weapon.ammo,
                                condition = weapon.condition
                            };

                            Destroy(weapon.gameObject);
                            first.Clear();
                            PlayerInventory.items[second.index] = state;
                            second.inventory.OnCellUpdate?.Invoke(second.index);
                        }
                    }
                    return;
                }
                if (second.type == InventoryCell.MyType.WeaponSlot)
                {
                    if (first.state is WeaponState)
                    {
                        if (second.state == null)
                        {
                            EquipWeapon(first);
                        }
                        else
                        {
                            var weapon = player.weapon.UnEquip();
                            var state = new WeaponState
                            {
                                data = weapon.data,
                                ammo = weapon.ammo,
                                condition = weapon.condition
                            };
                            Destroy(weapon.gameObject);
                            EquipWeapon(first);
                            PlayerInventory.items[first.index] = state;
                            first.inventory.OnCellUpdate?.Invoke(first.index);
                        }

                    }
                    //TODO
                    return;
                }
                //Перемещение в слот для ножа
                if (first.type == InventoryCell.MyType.KnifeSlot)
                {
                    //TODO
                    return;
                }
                if (second.type == InventoryCell.MyType.KnifeSlot)
                {
                    //TODO
                    return;
                }
                //Перемещение в слот для брони
                if (first.type == InventoryCell.MyType.ArmorSlot)
                {
                    if (second.state == null)
                    {
                        PlayerInventory.items[second.index] = player.UnEquipArmor();
                        second.inventory.OnCellUpdate?.Invoke(second.index);
                        first.Clear();
                    }
                    else
                    {
                        if (second.state is ArmorState)
                        {
                            var state = player.UnEquipArmor();
                            EquipArmor(second);
                            PlayerInventory.items[second.index] = state;
                            second.inventory.OnCellUpdate?.Invoke(second.index);
                        }
                    }
                    return;
                }
                if (second.type == InventoryCell.MyType.ArmorSlot)
                {
                    if (first.state is ArmorState)
                    {
                        if (second.state == null)
                        {
                            EquipArmor(first);
                        }
                        else
                        {
                            var state = player.UnEquipArmor();
                            EquipArmor(first);
                            PlayerInventory.items[first.index] = state;
                            first.inventory.OnCellUpdate?.Invoke(first.index);
                        }
                    }
                    return;
                }
                //Перемещение в слот продажи
                if ((first.type == InventoryCell.MyType.CustomerSlot) && (second.type == InventoryCell.MyType.SaleSlot))
                {
                    //Проверка, что не пытаемся продать непродаваемый предмет
                    if (((first.state != null) && ((first.state.data.Cost == 0) || first.state.data.questItem)) ||
                        ((second.state != null) && ((second.state.data.Cost == 0) || second.state.data.questItem)))
                        return;
                    Swap(first, second);
                    RecalculateSale();
                    return;
                }
                if ((second.type == InventoryCell.MyType.CustomerSlot) && (first.type == InventoryCell.MyType.SaleSlot))
                {
                    //Проверка, что не пытаемся продать непродаваемый предмет
                    if (((first.state != null) && ((first.state.data.Cost == 0) || first.state.data.questItem)) ||
                        ((second.state != null) && ((second.state.data.Cost == 0) || second.state.data.questItem)))
                        return;
                    Swap(first, second);
                    RecalculateSale();
                    return;
                }
                //Перемещение в слот покупки
                if ((first.type == InventoryCell.MyType.TraderSlot) && (second.type == InventoryCell.MyType.PurchaseSlot))
                {
                    //Проверка, что не пытаемся продать непродаваемый предмет
                    if (((first.state != null) && (first.state.data.Cost == 0)) ||
                        ((second.state != null) && (second.state.data.Cost == 0)))
                        return;
                    Swap(first, second);
                    RecalculatePurchase();
                    return;
                }
                if ((second.type == InventoryCell.MyType.TraderSlot) && (first.type == InventoryCell.MyType.PurchaseSlot))
                {
                    //Проверка, что не пытаемся продать непродаваемый предмет
                    if (((first.state != null) && (first.state.data.Cost == 0)) ||
                        ((second.state != null) && (second.state.data.Cost == 0)))
                        return;
                    Swap(first, second);
                    RecalculatePurchase();
                    return;
                }
                //Остальные случаи перемещения
                if (first.type == second.type)
                {
                    if (first.inventory != second.inventory)
                    {
                        if (first.inventory == PlayerInventory && first.state != null && first.state.data.questItem)
                            return;
                        if (second.inventory == PlayerInventory && second.state != null && second.state.data.questItem)
                            return;
                    }
                    Swap(first, second);
                    return;
                }
            }
        }
    }
    private void LateUpdate()
    {
        if (time > 0)
        {
            time = Mathf.Max(0, time - Time.deltaTime);
            if (time == 0)
            {
                double_click = null;
            }
        }
        if (first != null)
        {
            if (Input.GetMouseButton(0))
            {
                first.icon.transform.position = mainCamera.ScreenToWorldPoint(Input.mousePosition) + new Vector3(0, 0, 1) + (Vector3)first.icon_offset;
            }
            if (Input.GetMouseButtonUp(0))
            {
                first.icon.transform.position = first.transform.position;
                first.icon.transform.localPosition = first.icon_offset;
                first = null;
                second = null;
            }
        }
    }
}
