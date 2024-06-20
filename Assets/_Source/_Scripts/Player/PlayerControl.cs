using Game.Sounds;
using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.Rendering;
using UnityEngine.UI;

public class PlayerControl : MonoBehaviour, IUpdatable
{
    public Joystick moveStick;
    public Joystick lookStick;
    public TargetPointer targetPointer;
    public float direction = 0; //Направление взгляда

    [HideInInspector] public LookPoint lookPoint; //Точка, за которой следит CinemachineCamera

    private Vector3 prevPosition; //Предыдущая позиция на карте

    [HideInInspector] public Human human; //Человек, которым управляет игрок
    private Camera mainCamera;
    private CameraParams cameraParams;
    public Background background;

    public Damageable target;
    public ShopOpener shopOpener;

    [HideInInspector] public Rigidbody2D rigidBody2D;
    [HideInInspector] public Transform _transform;

    private bool shooting;

    public ProgressBar healthBar;
    public GameObject hpBar;
    public GameObject miniMap;
    public GameObject Map;
    public bool showInventory;
    public bool showJournal;
    public bool showHistory;
    public bool showDrop;
    public bool showDialog;
    public GameObject overlay;
    public GameObject inventory;
    public InformationWindow informationWindow;
    public InventoryWindow inventoryWindow;

    public GameObject torchLight;

    public bool inventory_is_open;

    public UnityEvent OnWindowChanged; //Событие смены окон

    public GameObject androidUI;
    public GameObject messages;

    //AudioClip
    public AudioClip stepClip;
    private bool stepBool = true;
    public AudioClip reloadClip;
    public AudioClip ammoNoneClip;
    public AudioClip medicineClip;

    float _timeToLookPosReset;
    bool _LookPosResetStarted;

    bool _hasFullHealth;
    public UnityEvent OnHealingStart;
    public UnityEvent OnHealingEnd;
    public UnityEvent OnFullHealth;
    public UnityEvent OnNotFullHealth;

    void Awake()
    {
        mainCamera = Camera.main;
        cameraParams = mainCamera.GetComponent<CameraParams>();
        human = GetComponent<Human>();
        rigidBody2D = human.GetComponent<Rigidbody2D>();
        _transform = GetComponent<Transform>();
        prevPosition = _transform.position;
        healthBar.player = human;
        human.OnMineralEquip.AddListener(healthBar.UpdateBar);
        human.OnMineralUnEquip.AddListener(healthBar.UpdateBar);
        healthBar.UpdateBar();
        healthBar.SetHealth(human.MaxHealth);
        human.OnHealthChanged.AddListener(healthBar.UpdateBar);
        inventory_is_open = false;
        human.DefaultValues(human.clan);
    }
    private void Start()
    {
        EventManager.Instance.OnTouchBegan.AddListener(OnScreenTouch);

        human.OnHealthChanged.AddListener(CheckFullHealth);
        if (human.Health == human.MaxHealth)
            _hasFullHealth = true;
    }

    private void CheckFullHealth()
    {
        if (_hasFullHealth && (human.Health != human.MaxHealth))
        {
            _hasFullHealth = false;
            OnNotFullHealth?.Invoke();
            return;
        }
        if (!_hasFullHealth && human.Health == human.MaxHealth)
        {
            _hasFullHealth = true;
            OnFullHealth?.Invoke(); 
            return;
        }
    }

    private void OnLookPosChanged()
    {
        _timeToLookPosReset = 0.5f;
        _LookPosResetStarted = false;
    }
    public void OnScreenTouch(Vector3 position)
    {
        if (showInventory)
        {
            ShowInventory();
        }
        else
        if (showHistory)
        {
            ShowHistory();
        }
        else
        if (showJournal)
        {
            ShowJournal();
        }
        else
        if (!showDialog)
        {
            TargetSearch.instance.Search(mainCamera.ScreenToWorldPoint(position));
        }
        //public bool showJournal;
        //public bool showHistory;
        //public bool showDrop;
        //public bool showDialog;)
    }

    public void ShowOverlay()
    {
        androidUI.SetActive(!androidUI.activeInHierarchy);
        messages.SetActive(!messages.activeInHierarchy);
        miniMap.SetActive(!miniMap.activeInHierarchy);
        hpBar.SetActive(!hpBar.activeInHierarchy);
    }

    public void ShowInventory()
    {
        if (!DialogController.instance.open)
        {
            if (!InventoryController.Instance.inventory)
            {
                InventoryController.Instance.InventoryOpen();
                showInventory = true;
                showHistory = false;
                showJournal = false;
            }
            else
            {
                InventoryController.Instance.InventoryClose();
                showInventory = false;
            }
            ShowOverlay();
            OnWindowChanged?.Invoke();
        }
    }
    public void ShowHistory()
    {
        if (!DialogController.instance.open)
        {
            if (HistoryController.instance.open)
            {
                HistoryController.instance.HistoryClose();
                showHistory = false;
            }
            else
            {
                HistoryController.instance.HistoryOpen();
                showHistory = true;
                showInventory = false;
                showJournal = false;
            }
            ShowOverlay();
            OnWindowChanged?.Invoke();
        }

    }
    public void ShowJournal()
    {
        if (!DialogController.instance.open)
        {
            if (TaskController.instance.open)
            {
                TaskController.instance.JournalClose();
                showJournal = false;
            }
            else
            {
                TaskController.instance.JournalOpen();
                showJournal = true;
                showInventory = false;
                showHistory = false;
            }
            ShowOverlay();
            OnWindowChanged?.Invoke();
        }
    }

    public void FastMedicine()
    {
        if (Command.HasItem(human.inventory, "bandage", 1))
        {
            Command.RemoveItem(human.inventory, "bandage");
            human.TakeHeal((Command.items["bandage"] as MedicineData).heal);
            AudioPlayer.Instance.PlaySoundFX(medicineClip, human.basicSource);
            Command.ShowMessage("Быстрое лечение", "Использован предмет: Бинт", Color.white);
        }
        else if (Command.HasItem(human.inventory, "medkitsmall", 1))
        {
            Command.RemoveItem(human.inventory, "medkitsmall");
            human.TakeHeal((Command.items["medkitsmall"] as MedicineData).heal);
            AudioPlayer.Instance.PlaySoundFX(medicineClip, human.basicSource);
            Command.ShowMessage("Быстрое лечение", "Использован предмет: Маленькая аптечка", Color.white);
        }
        else if (Command.HasItem(human.inventory, "medkitmedium", 1))
        {
            Command.RemoveItem(human.inventory, "medkitmedium");
            human.TakeHeal((Command.items["medkitmedium"] as MedicineData).heal);
            AudioPlayer.Instance.PlaySoundFX(medicineClip, human.basicSource);
            Command.ShowMessage("Быстрое лечение", "Использован предмет: Обычная аптечка", Color.white);
        }

    }

    //Кнопки для андроида
    public void ActionButton()
    {
        shopOpener.ActionButton();
    }
    public void ReloadButton()
    {
        if ((human.weapon != null) && (!human.weapon.reload))
        {
            if (human.Ammo[human.weapon.data.ammoType] == 0)
            {
                AudioPlayer.Instance.PlaySoundFX(ammoNoneClip, human.basicSource);
            }
            if ((human.weapon.ammo < human.weapon.data.clip) && (human.Ammo[human.weapon.data.ammoType] > 0))
            {
                AudioPlayer.Instance.PlaySoundFX(reloadClip, human.basicSource);
                human.weapon.ReloadBegin();
            }
        }
    }
    public void JournalButton()
    {
        ShowHistory();
    }
    public void MapButton()
    {
        Map.SetActive(!Map.activeInHierarchy);
    }
    public void TaskButton()
    {
        ShowJournal();
    }
    public void FlashlightButton()
    {
        torchLight.SetActive(!torchLight.activeSelf);
    }
    public void ShotButtonDown()
    {
        shooting = true;
    }
    public void ShotButtonUp()
    {
        shooting = false;
    }
    public void InventoryButton()
    {
        ShowInventory();
    }

    private void MovementLogic()
    {
        float moveHorizontal = moveStick.Horizontal();//Input.GetAxisRaw("Horizontal");
        float moveVertical = moveStick.Vertical();//Input.GetAxisRaw("Vertical");

        float lookHorizontal = lookStick.Horizontal(false);
        float lookVertical = lookStick.Vertical(false);
        Vector3 lookVector = new Vector3 (lookHorizontal, lookVertical);

        float lookMagnitude = lookVector.magnitude;

        // TODO PlayerControl замедление ходьбы при стрельбе
        bool shooting = false;//Input.GetMouseButton(0) && (human.weapon != null) &&(!DialogController.instance.open) && (!InventoryController.instance.inventory) && (!InventoryController.instance.trading) && (!InventoryController.instance.exchange);
        //Vector3 moveVector = new Vector3(moveHorizontal, moveVertical).normalized * Time.fixedDeltaTime * human.moveSpeed;
        Vector3 moveVector = human.moveSpeed * Time.fixedDeltaTime * new Vector3(moveHorizontal, moveVertical);
        if (moveVector.magnitude > 0)
        {
            //TODO PlayerControl фикс покачиваний и звука шагов в зависимости от скорости движения
            //human.swing += human.swingCoef * human.movement * Time.fixedDeltaTime;
            if (stepBool)
            {
                AudioPlayer.Instance.PlaySoundFX(stepClip, human.stepSource);
                stepBool = false;
                StartCoroutine(StepBool());
            }
            if (target == null)
            {
                if (!_LookPosResetStarted)
                {
                    if (_timeToLookPosReset > 0)
                    {
                        _LookPosResetStarted = true;
                    }
                    else
                    {
                        float angle = Mathf.Atan2(moveVector.y, moveVector.x) * Mathf.Rad2Deg;
                        Quaternion targetRotation = Quaternion.AngleAxis(angle, Vector3.forward);
                        transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, Time.fixedDeltaTime * human.rotationSpeed);
                    }
                }
            }
            //if (human.legs != null)
            //{
            //    human.legs.Rotate(Mathf.Sign(moveHorizontal) * Mathf.Abs(Mathf.Round(moveHorizontal)), Mathf.Sign(moveVertical) * Mathf.Abs(Mathf.Round(moveVertical)));
            //    human.legs.Show();
            //    human.legs.time -= Time.fixedDeltaTime * human.moveSpeed;
            //}
        }
        //else
        //{
        //    if (human.legs != null)
        //    {
        //        human.legs.Hide();
        //    }
        //}
        if (target == null && lookMagnitude > 0)
        {
            OnLookPosChanged();
            float angle = Mathf.Atan2(lookVector.y, lookVector.x) * Mathf.Rad2Deg;
            Quaternion targetRotation = Quaternion.AngleAxis(angle, Vector3.forward);
            transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, Time.fixedDeltaTime * human.rotationSpeed);
            lookPoint.SetPos(lookVector, lookVector.magnitude * lookPoint.maxDist);
        }
        if (shooting)
        {
            human.swing = 0;
            moveVector /= 1.5f;
        }
        if (moveVector != Vector3.zero)
        {
            rigidBody2D.AddForce(moveVector);
        }
    }

    public IEnumerator StepBool()
    {
        yield return Yielders.Get(0.5f);
        stepBool = true;
    }

    private void FixedUpdate()
    {
        MovementLogic();
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
        if (_LookPosResetStarted)
        {
            if (_timeToLookPosReset > 0f)
            {
                _timeToLookPosReset -= Time.deltaTime;
                if (_timeToLookPosReset <= 0f )
                {
                    _timeToLookPosReset = 0f;
                    _LookPosResetStarted = false;
                    lookPoint.ResetPos();
                }
            }
        }

        //human._transform.eulerAngles = new Vector3(0, 0, 90-direction + human.swing);
        var tr = (prevPosition - human._transform.position);
        //background.Move(tr[0], tr[1]);

        prevPosition = human._transform.position;

        //Взгляд на курсор
        //Vector3 cursor = mainCamera.ScreenToWorldPoint(Input.mousePosition) - human._transform.position;
        //direction = Mathf.Atan2(cursor.x, cursor.y) * Mathf.Rad2Deg;
        if (target != null)
        {
            OnLookPosChanged();
            var dir = target.transform.position - transform.position;
            float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
            Quaternion targetRotation = Quaternion.AngleAxis(angle, Vector3.forward);
            transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, Time.deltaTime * human.rotationSpeed);
            lookPoint.SetPos(dir, dir.magnitude);
            //human._transform.rotation = Quaternion.FromToRotation(Vector2.right, target.transform.position - transform.position);
        }
        //Передвижение

        //Стрельба
        //&& Input.GetMouseButton(0)
        if (shooting && (!InventoryController.Instance.inventory) && (!InventoryController.Instance.trading) && (!InventoryController.Instance.exchange) && (!DialogController.instance.open) && (!HistoryController.instance.open) && (!TaskController.instance.open) && (human.weapon != null))
        {
            //if (human.swing == 0) ;
            human.weapon.Shot();
        }
        //Перезарядка
        if (Input.GetKeyDown(KeyCode.R) && (human.weapon != null) && (!human.weapon.reload))
        {
            if (human.Ammo[human.weapon.data.ammoType] == 0)
            {
                AudioPlayer.Instance.PlaySoundFX(ammoNoneClip, human.basicSource);
            }
            if ((human.weapon.ammo < human.weapon.data.clip) && (human.Ammo[human.weapon.data.ammoType] > 0))
            {
                AudioPlayer.Instance.PlaySoundFX(reloadClip, human.basicSource);
                human.weapon.ReloadBegin();
            }
        }

        //Фонарик
        if (Input.GetKeyDown(KeyCode.L))
        {
            torchLight.SetActive(!torchLight.activeSelf);
        }

        //Инвентарь
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            ShowInventory();
        }

        //История диалогов
        if (Input.GetKeyDown(KeyCode.H))
        {
            ShowHistory();
        }

        //Журнад заданий
        if (Input.GetKeyDown(KeyCode.J))
        {
            ShowJournal();
        }

        //Убрать оружие
        if (Input.GetKeyDown(KeyCode.X))
        {
            if (human.weapon != null)
            {
                human.weapon.UnEquip().PickUp(human);
            }
        }

        //Аптечка
        if (Input.GetKeyDown(KeyCode.E))
        {
            FastMedicine();
        }

        //Карта
        //Map.SetActive(!showInventory && !showHistory && !showJournal && !showDrop && !showDialog && Input.GetKey(KeyCode.LeftShift));
    }
    private void OnDestroy()
    {
        Destroy(hpBar);
        Destroy(inventory);
        Destroy(overlay);
        Destroy(miniMap);
    }
}
