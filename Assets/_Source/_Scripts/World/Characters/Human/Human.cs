using Game.Sounds;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Events;

public class Human : Character
{
    public enum Skin { Noone, Default, Marauder, Madman, Scientist, Mechanic, MechLeader, MechQuest }
    public Skin skin;
    public string clan; //Группировка
    public bool questPerson; //Является ли важным персонажем
    public int money; //Деньги персонажа
    public int reputation; //Репутация
    public bool dropItemsOnDead; //Оставляет ли мешочек после смерти
    public SpriteRenderer headSprite;
    [HideInInspector] public Body body;
    public bool inCamp; //Находится ли сейчас в лагере
    public Dictionary<string, int> relation = new Dictionary<string, int>(); //Отношение к другим группировкам

    public Weapon weapon;
    public Knife knife;

    private readonly Dictionary<string, int> _ammo = new Dictionary<string, int>();
    public IReadOnlyDictionary<string, int> Ammo => _ammo; // Тип патронов - количество

    private readonly Dictionary<string, int> _medicine = new Dictionary<string, int>();
    public IReadOnlyDictionary<string, int> Medicine => _medicine;

    public Inventory inventory;

    public float damageCoef; //Коэффициент наносимого урона

    public float direction; //Направление взгляда
    public float swing; //Покачивание при движении
    public float swingCoef;
    public float maxSwing; //Максимальный угол покачивания

    public HumanLegs legs; //Ноги

    public ArmorData armorData; //Данные брони
    public float armorCondition; //Исправность брони брони

    public MineralData[] minerals = new MineralData[4]; //Минералы, размещенные на поясе

    #region UnityEvents
    [HideInInspector] public UnityEvent<Weapon> OnWeaponEquip; ///Событие взятия оружия в руки
    [HideInInspector] public UnityEvent OnArmorEquip; ///Событие надевания брони
    [HideInInspector] public UnityEvent<Knife> OnKnifeEquip; ///Событие взятия ножа в руки
    [HideInInspector] public UnityEvent OnMineralEquip; ///События добавления минерала на пояс

    [HideInInspector] public UnityEvent OnWeaponUnEquip; ///Событие удаления оружия из рук
    [HideInInspector] public UnityEvent OnArmorUnEquip; ///Событие снятия брони
    [HideInInspector] public UnityEvent OnKnifeUnEquip; ///Событие удаления ножа из рук
    [HideInInspector] public UnityEvent OnMineralUnEquip; ///Событие снятия минерала с пояса

    [HideInInspector] public UnityEvent OnArmorDamaged; ///Собыьте повреждения брони

    [HideInInspector] public UnityEvent OnShot; ///Событие выстрела из оружия
    [HideInInspector] public UnityEvent<string, int> OnReload; ///Событие перезарядки оружия

    [HideInInspector] public UnityEvent OnChangedMoney; ///События изменения количества денег у персонажа
    [HideInInspector] public UnityEvent<string> OnTakeAmmo; ///Событие изменения количества патронов у персонажа
    [HideInInspector] public UnityEvent<string> OnTakeMedicine; ///Событие изменения количества медикаментов у персонажа
    #endregion

    //AudioClip
    public AudioClip takeDamageMainHeroClip;
    public AudioClip ammoNoneClip;
    public bool takeDamageBlackout = true;

    public void TakeMoney(int n)
    {
        money += n;
        OnChangedMoney?.Invoke();
    }
    public ArmorState UnEquipArmor()
    {
        if (armorData != null)
        {
            var state = new ArmorState
            {
                data = armorData,
                condition = armorCondition
            };
            defence -= armorData.armorBonus * armorCondition / armorData.conditionMax;
            movement -= armorData.movementBonus;
            MovespeedRecalculate();
            armorCondition = 0;
            armorData = null;
            OnArmorUnEquip?.Invoke();
            return state;
        }
        else
        {
            return null;
        }
    }
    public void EquipArmor(ArmorData data, float condition)
    {
        armorData = data;
        armorCondition = condition;
        movement += data.movementBonus;
        MovespeedRecalculate();
        defence += data.armorBonus * condition / data.conditionMax;
        OnArmorEquip?.Invoke();
    }
    public void EquipMineral(MineralData data, int slot)
    {
        if (minerals[slot] == null)
        {
            minerals[slot] = data;
            SetMaxHealth(_maxHealth + data.hpBonus);
            regen += data.regenBonus;
            defence += data.armorBonus;
            movement += data.movementBonus;
            MovespeedRecalculate();
            OnMineralEquip?.Invoke();
        }
    }
    public MineralState UnEquipMineral(int slot)
    {
        if (minerals[slot] != null)
        {
            var state = new MineralState
            {
                data = minerals[slot]
            };
            SetMaxHealth(_maxHealth - minerals[slot].hpBonus);
            regen -= minerals[slot].regenBonus;
            defence -= minerals[slot].armorBonus;
            movement -= minerals[slot].movementBonus;
            minerals[slot] = null;
            MovespeedRecalculate();
            OnMineralUnEquip?.Invoke();
            return state;
        }
        return null;
    }
    public void DefaultValues(string my_clan)
    {
        relation.Clear();
        relation.Add("Одиночки", 0);
        relation.Add("Мародеры", 0);
        relation.Add("Сумасшедшие", 0);
        relation.Add("Механики", 0);
        relation.Add("Торговцы", 0);
        relation.Add("Ученые", 0);
        clan = my_clan;
        reputation = 0;
        switch (clan)
        {
            case "Одиночки":
                {
                    if (skin == Skin.Noone)
                        body.ChangeSkin(body.DefaultSkin);
                    if (MyName == "")
                        MyName = "Одиночка";
                    relation["Одиночки"] = 1000;
                    relation["Мародеры"] = -1000;
                    relation["Сумасшедшие"] = -1500;
                    relation["Механики"] = 500;
                    relation["Торговцы"] = 250;
                    relation["Ученые"] = 500;
                    break;
                }
            case "Мародеры":
                {
                    if (skin == Skin.Noone)
                        body.ChangeSkin(body.MarauderSkin);
                    if (MyName == "")
                        MyName = "Мародер";
                    relation["Одиночки"] = -1000;
                    relation["Мародеры"] = 1000;
                    relation["Сумасшедшие"] = -1500;
                    relation["Механики"] = -500;
                    relation["Торговцы"] = 0;
                    relation["Ученые"] = -500;
                    break;
                }
            case "Торговцы":
                {
                    if (MyName == "")
                        MyName = "Торговец";
                    relation["Одиночки"] = 250;
                    relation["Мародеры"] = 0;
                    relation["Сумасшедшие"] = 0;
                    relation["Механики"] = 250;
                    relation["Торговцы"] = 1000;
                    relation["Ученые"] = 250;
                    break;
                }
            case "Механики":
                {
                    if (skin == Skin.Noone)
                        body.ChangeSkin(body.MechanicSkin);
                    if (MyName == "")
                        MyName = "Механик";
                    relation["Одиночки"] = 500;
                    relation["Мародеры"] = -1000;
                    relation["Сумасшедшие"] = -2000;
                    relation["Механики"] = 1000;
                    relation["Торговцы"] = 250;
                    relation["Ученые"] = 500;
                    break;
                }
            case "Сумасшедшие":
                {
                    if (skin == Skin.Noone)
                        body.ChangeSkin(body.MadmanSkin);
                    if (MyName == "")
                        MyName = "Сумасшедший";
                    relation["Одиночки"] = -1500;
                    relation["Мародеры"] = -1500;
                    relation["Сумасшедшие"] = 1000;
                    relation["Механики"] = -2000;
                    relation["Торговцы"] = -1500;
                    relation["Ученые"] = -1500;
                    break;
                }
            case "Ученые":
                {
                    if (skin == Skin.Noone)
                        body.ChangeSkin(body.ScientistSkin);
                    if (MyName == "")
                        MyName = "Ученый";
                    relation["Одиночки"] = 500;
                    relation["Мародеры"] = -1500;
                    relation["Сумасшедшие"] = -1000;
                    relation["Механики"] = 500;
                    relation["Торговцы"] = 250;
                    relation["Ученые"] = 1000;
                    break;
                }
        }
        switch (skin)
        {
            case Skin.Default: { body.ChangeSkin(body.DefaultSkin); } break;
            case Skin.Marauder: { body.ChangeSkin(body.MarauderSkin); } break;
            case Skin.Madman: { body.ChangeSkin(body.MadmanSkin); } break;
            case Skin.Scientist: { body.ChangeSkin(body.ScientistSkin); } break;
            case Skin.Mechanic: { body.ChangeSkin(body.MechanicSkin); } break;
            case Skin.MechLeader: { body.ChangeSkin(body.MechLeader); } break;
            case Skin.MechQuest: { body.ChangeSkin(body.MechQuest); } break;
        }
    }
    public void TakeAmmo(string type, int count)
    {
        _ammo[type] = math.max(0, _ammo.TryGetValue(type, out var existingCount) ? existingCount + count : count);
        if (count == 0) return;
        OnTakeAmmo?.Invoke(type);
    }
    public void TakeMedicine(string type, int count)
    {
        _medicine[type] = math.max(0, _medicine.TryGetValue(type, out var existingCount) ? existingCount + count : count);
        if (count == 0) return;
        OnTakeMedicine?.Invoke(type);
    }
    private void TakeDamageToArmor(float damage)
    {
        //Ухудшение состояния брони и снижение показателя защиты
        if ((armorData != null) && (armorCondition > 0))
        {
            OnArmorDamaged?.Invoke(); //TODO почему событие вызывается дважды?
            var t = armorCondition;
            armorCondition = Mathf.Max(0, armorCondition - damage);
            defence -= armorData.armorBonus * (t - armorCondition) / armorData.conditionMax;
            OnArmorDamaged?.Invoke();
        }
    }
    public override void TakeDamage(float damage, Damageable damager = null, DamageType damageType = DamageType.Default)
    {
        base.TakeDamage(damage, damager);
        if (damager != null && damageType != DamageType.Pure)
        {
            if (mainHero)
            {
                AudioPlayer.Instance.PlaySoundFXWithRandomPitch(takeDamageMainHeroClip, basicSource);
            }
            TakeDamageToArmor(damage);
        }
        else // Урон нанесен затмением
        {
            if (mainHero && takeDamageBlackout)
            {
                AudioPlayer.Instance.PlaySoundFXWithRandomPitch(takeDamageMainHeroClip, basicSource);
                takeDamageBlackout = false;
                StartCoroutine(TakeDamageBlackout());
            }
        }
    }
    public IEnumerator TakeDamageBlackout()
    {
        yield return Yielders.Get(2);
        takeDamageBlackout = true;
    }

    public override void Awake()
    {
        base.Awake();
        DefaultValues(clan);
        if (MyName == "random")
        {
            MyName = Command.RandomName();
        }
    }
    private void Start()
    {
        damageCoef = 1;
        body = GetComponentInChildren<Body>();
        if (armorData != null)
        {
            movement += armorData.movementBonus;
            MovespeedRecalculate();
            defence += armorData.armorBonus * armorCondition / armorData.conditionMax;
        }
    }
    public override void Death(Damageable killer = null)
    {
        if (legs != null)
            Destroy(legs.gameObject);
        if (dropItemsOnDead)
        {
            var drop = Instantiate(Prefabs.dropItems, new Vector3(this.transform.position.x, transform.position.y, transform.position.z), Quaternion.identity);
            //InventoryController.instance.OnInventoryClose.AddListener(drop.GetComponent<DropItems>().showButoon);
            //InventoryController.instance.OnInventoryOpen.AddListener(drop.GetComponent<DropItems>().hideButoon);
            inventory.transform.SetParent(drop.transform);
            drop.inventory = inventory;
            drop.inventory.deadName = MyName;
            drop.money = money / 10;
            drop.firstTime = true;
            if (questPerson)
            {
                drop.toDestroy = true;
            }
            inventory.place = drop.gameObject;
            inventory.owner = null;
        }
        base.Death(killer);
    }

    public void OnCollisionEnter2D(Collision2D collision)
    {
        if (ai)
        {
            if (ai.move)
            {
                //TODO найти более эффективный метод обхода препятствий
                if (!collision.gameObject.TryGetComponent(out Bullet _) && !collision.gameObject.TryGetComponent(out Shelter _))
                {
                    //print(collision.gameObject.name);
                    var x = (ai as HumanAI);
                    if (Vector2.Distance(_transform.position, x.movePosition) < 32)
                    {
                        x.move = false;
                    }
                    else
                    {
                        Vector2 A = (x.movePosition);
                        Vector2 B = (_transform.position);
                        Vector2 C = collision.contacts[0].point;
                        int r = -1;

                        if ((C.x - A.x) * (B.y - A.y) - (C.y - A.y) * (B.x - A.x) < 0) //Определение, с какой стороны летит пуля 
                        {
                            r = 1;
                        }
                        x.movePosition = _transform.position + Quaternion.Euler(0, 0, 90 * r) * x.moveVector.normalized * 32;
                        //rigidBody.AddForce(Quaternion.Euler(0, 0, 90 * r) * _transform.right * 30000 * 90); //Уворот от пули
                        //x.movePosition = collision.contacts[0].point;
                    }
                }
            }
        }
    }

    public override void ManualUpdate()
    {
        if (Mathf.Abs(swing) > maxSwing)
        {
            swing = Mathf.Sign(swingCoef) * maxSwing;
            swingCoef *= -1;
        }
        if (Time.frameCount % 13 == 0)
            UpdateCurrentChunk();
        base.ManualUpdate();
    }
    private void UpdateCurrentChunk()
    {
        var coord = Chunks.CalculateCurrentChunk(transform.position);
        if (coord != currentChunk)
        {
            //Logger.Debug($"ChunkUpdated {_myName}");
            Chunks.ChangeChunk(ref currentChunk, ref coord, this);
        }
    }
    protected override void AddEntityToWorld()
    {
        Entities.Add(this);
        currentChunk = Chunks.CalculateCurrentChunk(transform.position);
        Chunks.Add(currentChunk ,this);
    }

    protected override void RemoveEntityFromWorld()
    {
        Entities.Remove(this);
        Chunks.Remove(currentChunk, this);
    }
}
