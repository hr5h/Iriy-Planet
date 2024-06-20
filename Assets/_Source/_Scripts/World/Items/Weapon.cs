using Game.Sounds;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;

public class Weapon : Item
{
    public WeaponData data;
    private Material default_material;
    [HideInInspector] public Human owner; //Владелец оружия
    public Sprite skin; //Спрайт оружия
    [HideInInspector] public BoxCollider2D boxCollider;
    public Transform shotPlace; //Дуло оружия, место, где создается пуля

    public int ammo; //Патроны в обойме
    [HideInInspector] public float reloadCurrent = 0; //Текущее время перезарядка
    [HideInInspector] public bool reload = false; //Перезарядка идёт
    public int condition; //Текущая исправность
    [HideInInspector] public float spreadCurrent; //Текущий разброс пуль
    [HideInInspector] public float rateCurrent; //Текущий интервал между выстрелами
    [HideInInspector] public bool shot; //Выстрел совершен
    private float bodyTimeout; //Промежуток времени, после которого тело персонажа изменит спрайт
    [HideInInspector] public ParticleSystem firePoint;

    //private Inventory inventory;
    [HideInInspector] public Vector2 length;

    private bool shootAudioBool = true;

    [HideInInspector] public UnityEvent OnShot;

    [HideInInspector] public UnityEvent OnReloadBegin;
    [HideInInspector] public UnityEvent OnReloadCancel;
    [HideInInspector] public UnityEvent OnReloadEnd;

    protected override void Awake()
    {
        boxCollider = GetComponent<BoxCollider2D>();
        firePoint = GetComponentInChildren<ParticleSystem>();
        base.Awake();
    }
    public void Init(WeaponState state) //Для инициализации из инвентаря
    {
        condition = state.condition;
        ammo = state.ammo;
        data = state.data as WeaponData;
        var col = firePoint.colorOverLifetime;
        col.color = new ParticleSystem.MinMaxGradient(data.firePointColor);
        Init();
    }
    public override void Init()
    {
        if (!data)
        {
            Logger.Debug("Нет данных об оружии!");
            return;
        }
        var width = data.Icon.bounds.size.x;
        var height = data.Icon.bounds.size.y;
        var center = data.Icon.pivot;

        name = data.Title;

        boxCollider.size = new Vector2(width, height); //TODO поправить shotPlace с учетом смещения центра спрайта

        spriteRenderer.sprite = data.Icon;
        spreadCurrent = data.spreadMin;
        reload = false;
        bodyTimeout = 0;
        reloadCurrent = 0;

        shotPlace.localPosition = new Vector2(width / 2, 0);
        length = shotPlace.localPosition;

        var col = firePoint.colorOverLifetime;
        col.color = new ParticleSystem.MinMaxGradient(data.firePointColor);

        owner = null;
        boxCollider.enabled = true;
        //rigidBody.simulated = true;

        if (ammo > data.clip) ammo = data.clip;
        if (condition > data.conditionMax) condition = data.conditionMax;
        default_material = spriteRenderer.sharedMaterial;
        base.Init();
    }
    public override bool PickUp(Human human)
    {
        if (human.inventory.IsEmpty())
        {
            var state = new WeaponState
            {
                data = data,
                ammo = ammo,
                condition = condition
            };

            if (human.inventory.Add(state))
            {
                Destroy(gameObject);
                return true;
            }
        }
        return false;
    }
    protected override void CollisionHandler(Collider2D collision)
    {
        if (owner == null)
        {
            if (collision.TryGetComponent(out Human human))
            {
                if (!human.mainHero && data.dontPickup)
                    return;
                if (human && human.mainHero)
                {
                    Command.ShowMessage("Получен предмет:", data.Title, Color.white, 1, false);
                }
                if (human.weapon == null)
                {
                    Equip(human);
                }
                else
                {
                    PickUp(human);
                }
            }
        }
    }
    public void ReloadBegin() //Начать перезарядку
    {
        if (owner != null)
        {
            bodyTimeout = 0;
            reload = true;
            reloadCurrent = data.reloadTime;
            firePoint.Clear();
            firePoint.Stop();
            owner.body.Reload();
            OnReloadBegin?.Invoke();
        }
        else
        {
            ReloadCancel();
        }
    }
    public void ReloadCancel() //Прервать перезарядку
    {
        reload = false;
        reloadCurrent = 0;
        if (owner != null)
        {
            owner.body.StayWeapon();
            OnReloadCancel?.Invoke();
        }
    }
    public void ReloadEnd() //Завершить перезарядку
    {
        if (owner != null)
        {
            owner.OnReload.Invoke(data.ammoType, Mathf.Min(data.clip - ammo, owner.Ammo[data.ammoType]));
            var tempAmmo = ammo;
            ammo = Mathf.Min(data.clip, owner.Ammo[data.ammoType] + ammo);
            owner.TakeAmmo(data.ammoType, -data.clip + tempAmmo);
            OnReloadEnd?.Invoke();
        }
        ReloadCancel();
    }
    public void Shot() //Выстрелить
    {
        if ((owner != null) && !shot && (condition > 0))
        {
            if (!reload)
            {
                if (owner.mainHero && ammo == 0 && shootAudioBool)
                {
                    AudioPlayer.Instance.PlaySoundFX(owner.ammoNoneClip, owner.weaponSource);
                    shootAudioBool = false;
                    StartCoroutine(NoneAmmoAudio());
                }
                if (ammo > 0)
                {
                    AudioPlayer.Instance.PlaySoundFX(data.shootAudio, owner.weaponSource);
                    owner.body.Shoot();
                    ammo--;
                    condition--; //TODO Влияние износа оружия на точность стрельбы (возможно заклинивание)
                    shot = true;
                    rateCurrent = data.rate;

                    firePoint.Play();



                    if (data.IsShotgun)
                    {
                        for (int i = 0; i < 7; i++)
                        {
                            var bullet = worldController.SpawnBullet(shotPlace.position, transform.rotation, data.bulletColor, data.bulletWidth, data.bulletTrailTime, data.bulletMass);
                            bullet.transform.eulerAngles += new Vector3(0, 0, Random.Range(-spreadCurrent, spreadCurrent));
                            bullet.owner = owner;
                            bullet.Init(data);
                        }
                    }
                    else
                    {
                        var bullet = worldController.SpawnBullet(shotPlace.position, transform.rotation, data.bulletColor, data.bulletWidth, data.bulletTrailTime, data.bulletMass);
                        bullet.transform.eulerAngles += new Vector3(0, 0, Random.Range(-spreadCurrent, spreadCurrent));
                        bullet.owner = owner;
                        bullet.Init(data);


                    }
                    spreadCurrent = Mathf.Min(data.spreadMax, spreadCurrent + data.spreadIncreace);
                    owner.OnShot?.Invoke();
                    OnShot?.Invoke();
                }
                else
                {
                    if (owner.Ammo[data.ammoType] > 0)
                        ReloadBegin();
                }
            }
        }
    }

    public Weapon Equip(Human human) //Экипировать
    {
        spriteRenderer.sharedMaterial = default_material;
        human.weapon = this;
        owner = human;
        owner.movement -= data.movementCoef;
        owner.MovespeedRecalculate();
        owner.body.StayWeapon();
        boxCollider.enabled = false;
        //rigidBody.simulated = false;
        _transform.SetParent(owner.transform);

        if (!owner.Ammo.ContainsKey(data.ammoType))
        {
            owner.TakeAmmo(data.ammoType, 0);
        }

        bodyTimeout = 0;
        if (owner.ai != null)
        {
            (owner.ai as HumanAI).noWeapon = false;
            (owner.ai as HumanAI).weaponLength += length;
        }
        if (owner.mainHero)
        {
            worldController.weaponUI.active = true;
        }
        if (timer) //Отменить таймер самоуничтожения
            DestroyTimerCancel();
        owner.OnWeaponEquip?.Invoke(this);
        return this;
    }

    public Weapon UnEquip() //Снять
    {
        if (owner != null)
        {
            owner.movement += data.movementCoef;
            owner.MovespeedRecalculate();
            owner.body.Stay();
            owner.OnWeaponUnEquip?.Invoke();
        }
        if (owner.ai != null)
        {
            (owner.ai as HumanAI).noWeapon = true;
            (owner.ai as HumanAI).weaponLength -= length;
        }
        if (owner.mainHero)
        {
            worldController.weaponUI.active = false;
            worldController.weaponUI.ClearText();
        }
        owner.weapon = null;
        owner = null;
        boxCollider.enabled = true;
        //rigidBody.simulated = true;
        reload = false;
        reloadCurrent = 0;
        bodyTimeout = 0;
        //float rndz1 = Random.value;
        //float rndz2 = Random.value;
        //int z1;
        //int z2;
        //if (rndz1 < 0.5) z1 = 1;
        //else z1 = -1;
        //if (rndz2 < 0.5) z2 = 1;
        //else z2 = -1;
        _transform.SetParent(null);
        //_transform.position = new Vector2(transform.position.x + z1 * 100, transform.position.y + z2 * 100);
        return this;
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
    public override void ManualUpdate()
    {
        base.ManualUpdate();
        if (shot && (rateCurrent > 0))
        {
            rateCurrent = Mathf.Max(0, rateCurrent - Time.deltaTime);
            if (rateCurrent == 0) { shot = false; bodyTimeout = data.rate + 0.05f; }
        }
        if ((bodyTimeout > 0) && (owner != null))
        {
            bodyTimeout = Mathf.Max(0, bodyTimeout - Time.deltaTime);
            if (bodyTimeout == 0)
            {
                owner.body.StayWeapon();
            }
        }
        if (reload && (reloadCurrent > 0))
        {
            if (owner == null) ReloadCancel();
            reloadCurrent = Mathf.Max(0, reloadCurrent - Time.deltaTime);
            if (reloadCurrent == 0) ReloadEnd();
        }
        if (spreadCurrent > data.spreadMin)
        {
            spreadCurrent = Mathf.Max(data.spreadMin, spreadCurrent - data.spreadReduction * Time.deltaTime);
        }
    }

    public IEnumerator NoneAmmoAudio()
    {
        yield return Yielders.Get(1);
        shootAudioBool = true;
    }
    public override void DestroyTimerStart(float t = 60)
    {
        if (!data.questItem)
        {
            base.DestroyTimerStart(t);
        }
    }
}
