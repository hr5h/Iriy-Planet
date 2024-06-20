using System.Collections;
using UnityEngine;

//[RequireComponent(typeof(Image))]
public class Item : MonoBehaviour, IUpdatable
{
    private EntityRepository Entities => WorldController.Instance.Entities;

    [HideInInspector] public Transform _transform;
    [HideInInspector] public Rigidbody2D rigidBody;
    [HideInInspector] public SpriteRenderer spriteRenderer;
    [HideInInspector] public WorldController worldController;

    [HideInInspector] public bool init;
    public bool timer; //Запущен ли таймер уничтожения
    public float timeToDestroy; //Сколько осталось времени до уничтожения

    private Vector2Int _currentChunk;

    [ContextMenu("Инициализация")]
    public void EditorAwake()
    {
        Logger.Debug("Инициализация выполнена");
        Awake();
        Start();
    }
    public virtual void Init()
    {
        init = true;
    }
    public virtual bool PickUp(Human human)
    {
        return false;
    }
    // OnTriggerEnter2D не хотел корректно работать, будучи виртуальным, поэтому создал этот метод
    protected virtual void CollisionHandler(Collider2D collision) 
    {
        if (collision.TryGetComponent(out Human human))
        {
            PickUp(human);
        }
    }

    protected void OnTriggerEnter2D(Collider2D collision)
    {
        CollisionHandler(collision);
    }
    protected virtual void Awake()
    {
        _transform = GetComponent<Transform>();
        rigidBody = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        worldController = GameObject.FindGameObjectWithTag("WorldController").GetComponent<WorldController>();
        init = false;
    }
    protected virtual void Start()
    {
        if (!init)
        {
            Init();
        }
    }

    //Чтобы бесхозные неквестовые предметы не лежали слишком долго на карте, уничтожаем их по таймеру
    public virtual void DestroyTimerStart(float t = 60f) //Запуск таймера самоуничтожения
    {
        //TODO плохо что пришлось сделать эту функцию виртуальной и просто продублировать код из-за отсутствия ItemData в этом классе
        //Но если добавить сюда ItemData, то в дочерних классах будет две data
        timer = true;
        timeToDestroy = t;
    }
    private void DestroyTimerFinish() //Окончание таймера самоуничтожения
    {
        Destroy(gameObject);
    }
    public void DestroyTimerCancel() //Отмена таймера самоуничтожения
    {
        timer = false;
    }

    public virtual void ManualUpdate()
    {
        if (timer)
        {
            timeToDestroy = Mathf.Max(0, timeToDestroy - Time.deltaTime);
            if (timeToDestroy == 0)
            {
                DestroyTimerFinish();
            }
        }
    }

    private void OnEnable() => StartCoroutine(OnEnableCoroutine());
    private IEnumerator OnEnableCoroutine() //Так как OnEnable данного класса может вызваться раньше, чем Awake класса WorldController, то ожидаем один кадр
    {
        yield return null;
        WorldController.Instance.Updater.Add(this);
        _currentChunk = WorldController.Instance.ChunkManager.CalculateCurrentChunk(transform.position);
        WorldController.Instance.ChunkManager.Add(_currentChunk, this);
        Entities.Add(this);
    }
    private void OnDisable()
    {
        WorldController.Instance.Updater.Remove(this);
        WorldController.Instance.ChunkManager.Remove(_currentChunk, this);
        Entities.Remove(this);
    }
}
