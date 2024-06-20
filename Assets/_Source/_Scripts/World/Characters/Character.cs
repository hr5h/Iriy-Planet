using Game.Sounds;
using System.Collections;
using UnityEngine;

public class Character : Damageable, IUpdatable//, IPointerDownHandler
{
    protected UpdateManager Updater => WorldController.Instance.Updater;
    protected EntityRepository Entities => WorldController.Instance.Entities;

    public GameObject AIPrefab;
    public string dropItem; //Предмет, который выпадает при смерти персонажа
    public float regen; //Регенерация
    public float defence; //Блокировка урона
    public float movement; //Показатель скорости передвижения
    public float moveSpeed; //Реальная скорость передвижения с учетом физической массы
    public float visionRange; //Дальность видимости (Для искусственного интеллекта)
    public float rotationSpeed; //Скорость поворота персонажа
    public Color bloodColor;
    public AI ai;
    public bool mainHero;
    public bool notForSpawner; //Не использовать спавнером
    public PlayerControl player;

    public SpriteRenderer rend;
    public Collider2D colliderObject;

    public Dialog dialog;
    public bool canSpeak;
    public bool canTrade;

    [HideInInspector] public Transform _transform; //Кэшированный transform
    [HideInInspector] public Rigidbody2D rigidBody;
    [HideInInspector] public WorldController worldController;

    //AudioSource
    public AudioSource basicSource;
    public AudioSource stepSource;
    public AudioSource weaponSource;

    //AudioClip
    public AudioClip deadClip;

    /// <summary>
    /// Расстояние до игрока
    /// </summary>
    public float Range(MonoBehaviour Player)
    {
        var CharacterPos = this.GetComponent<Transform>().position;
        var PlayerPos = Player.GetComponent<Transform>().position;
        return Mathf.Sqrt((CharacterPos.x - PlayerPos.x) * (CharacterPos.x - PlayerPos.x) + (CharacterPos.y - PlayerPos.y) * (CharacterPos.y - PlayerPos.y));
    }
    public virtual void Awake()
    {
        rigidBody = GetComponent<Rigidbody2D>();
        rend = GetComponent<SpriteRenderer>();
        colliderObject = GetComponent<Collider2D>();
        _transform = GetComponent<Transform>();
        if (rigidBody != null)
        {
            moveSpeed = 820 * rigidBody.mass * movement;
        }
        else
        {
            moveSpeed = 0;
        }
        worldController = worldController = GameObject.FindGameObjectWithTag("WorldController").GetComponent<WorldController>();
        //alpha = 0;
    }
    public void MovespeedRecalculate()
    {
        moveSpeed = 820 * rigidBody.mass * movement;
    }
    public virtual float DamageAfterResist(float damage, DamageType damageType = default)
    {
        if (damageType == DamageType.Pure)
            return defence < 100 ? damage : 0;
        return damage * (1 - defence / 100);
    }
    public override void TakeDamage(float damage, Damageable damager = null, DamageType damageType = DamageType.Default) //Получение урона
    {
        if (!IsDead && !gameObject.activeInHierarchy)
            return;
        damage = DamageAfterResist(damage, damageType);
        base.TakeDamage(damage, damager);
    }
    public override void Death(Damageable killer = null) //Смерть
    {
        if (ai != null)
            Destroy(ai.gameObject);
        Destroy(gameObject);
        AudioPlayer.Instance.PlaySoundFX(deadClip, transform.position, 0.1f);
        worldController.SpawnBloodBurst(_transform.position, bloodColor);
        worldController.SpawnBloodPool(_transform.position, bloodColor);
        if (dropItem != "")
        {
            Command.CreateItem(dropItem, transform.position, Quaternion.Euler(0, 0, Random.Range(0, 360)))[0].GetComponent<Item>().DestroyTimerStart();
        }
        base.Death(killer);
    }


    [ContextMenu("Наделить существо искусственным интеллектом")]
    public void CreateAI()
    {
        if (AIPrefab != null)
        {
            if (ai == null)
            {
                var obj = Instantiate(AIPrefab, transform.position, Quaternion.Euler(0, 0, 0)).GetComponent<AI>();
                obj.creature = this;
                ai = obj;
                Logger.Debug($"ИИ для объекта {name} создан!");
            }
            else
            {
                Logger.Debug($"Существо {name} уже обладает ИИ!");
            }
        }
        else
        {
            Logger.Debug($"Отсутствует ссылка на префаб ИИ для {name}!");
        }
    }

    public virtual void ManualUpdate()
    {
        if (regen > 0) TakeHeal(regen * Time.deltaTime);
    }

    private void OnEnable() => StartCoroutine(OnEnableCoroutine());
    private IEnumerator OnEnableCoroutine() //Так как OnEnable данного класса может вызваться раньше, чем Awake класса WorldController, то ожидаем один кадр
    {
        yield return null;
        Updater.Add(this);
        AddEntityToWorld();
        if (ai != null)
        {
            ai.gameObject.SetActive(true);
        }
    }
    private void OnDisable()
    {
        Updater.Remove(this);
        RemoveEntityFromWorld();
        if (ai != null)
        {
            ai.gameObject.SetActive(false);
        }
    }

    protected virtual void AddEntityToWorld() {}
    protected virtual void RemoveEntityFromWorld() {}
}
