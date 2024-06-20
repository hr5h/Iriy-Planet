using Game.Sounds;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using ObjectPooling;
using AddressableTools;

public class WorldController : MonoBehaviour, IUpdatable
{
    public static WorldController Instance { get; private set; }

    public Flashlight flashlight; //Вспышка на экране
    public PlayerControl playerControl; //Позиция игрока
    public Vector2Int currentChunk; //Координаты текущего чанка
    public int chunkSize; //Размер чанка
    public int chunkCounter;

    public EntityRepository Entities { get; private set; }
    public UpdateManager Updater { get; private set; }
    public ChunkManager ChunkManager { get; private set; }

    public PoolStorage poolStorage;

    //Глобальные состояния
    public bool blackout; //Происходит ли затмение
    public GameObject arrow;

    //UI игрока
    public CoordinatesUI coordinatesUI;
    public WeaponUI weaponUI;

    //AudioSource
    public AudioSource audioSource;
    public AudioSource thunderSource;
    public AudioSource bloodBurstSource;
    public AudioSource daySoundsSource;
    //AudioClip
    public AudioClip worldTheme;
    public AudioClip bloodBurstClip;
    public AudioClip startBlackoutClip;
    public AudioClip finishBlackoutClip;
    public AudioClip thunderClip;
    public AudioClip daySoundsClip;
    public AudioClip nightSoundsClip;

    private StringBuilder stringBuilder = new StringBuilder();

    [SerializeField] private AssetReferenceT<Sprite> stoneSpriteRefecence;
    [SerializeField] private AssetReferenceT<Sprite> plantSpriteRefecence;

    public Bullet SpawnBullet(Vector3 position, Quaternion rotation, Color color, float width = 5, float time = 0.1f, float mass = 0.0001f) //Специальный метод для создания пуль
    {
        var bullet = poolStorage.bulletData.pool.Get();
        bullet.Init(position, rotation, color, width, time, mass);
        return bullet;
    }
    public void SpawnBloodPool(Vector2 position, Color color) => 
        poolStorage.bloodPoolData.pool.Get().Init(position, color);
    public void SpawnBloodBurst(Vector2 position, Color color) //Специальный метод для создания взрыва крови
    {
        AudioPlayer.Instance.PlaySoundFX(bloodBurstClip, position, 0.3f);
        poolStorage.bloodBurstData.pool.Get().Init(position, color);
    }

    public void SpawnBloodSplash(Vector2 position, Color color) =>
        poolStorage.bloodSplashData.pool.Get().Init(position, color);

    public void SpawnBarrierSplash(Vector2 position, Color color) => 
        poolStorage.barrierSplashData.pool.Get().Init(position, color);

    public Dictionary<Vector2Int, Chunk> chunks = new Dictionary<Vector2Int, Chunk>(); //Словарь координаты-чанк
    public Stone SpawnStone(Vector3 position, float rotation, Sprite sprite) //Специальный метод для создания камней
    {
        var @object = poolStorage.stoneData.pool.Get();
        @object.Init(position, rotation, sprite);
        return @object;
    }
    public Plant SpawnPlant(Vector3 position, float rotation, Sprite sprite, Color col) //Специальный метод для создания растений
    {
        var @object = poolStorage.plantData.pool.Get();
        @object.Init(position, rotation, sprite);
        return @object;
    }

    //Проверка, находится ли точка в квестовой зоне
    public bool IsQuestArea(Vector2 pos)
    {
        var hit = Physics2D.Raycast(pos, Vector2.zero, 1, (1 << 12)).collider;
        return (hit != null);
    }
    public Chunk AddChunk(Vector2Int vector) //Добавление чанка
    {
        var obj = new Chunk(chunkSize, vector);
        chunks.Add(vector, obj);
        ++chunkCounter;
        return obj;
    }
    public List<Sprite> stones = new List<Sprite>(); //Спрайты камней
    public List<Sprite> plants = new List<Sprite>(); //Спрайты растений

    public void LoadResources() //Загрузка ресурсов
    {
        AddressableLoader.LoadAssetSpiteSheet(stoneSpriteRefecence, stones);
        AddressableLoader.LoadAssetSpiteSheet(plantSpriteRefecence, plants);
    }
    private void OnDestroy()
    {
        Application.targetFrameRate = 30;
        poolStorage.ReleasePools();
        Prefabs.Release();
    }
    private void InitManagers()
    {
        Entities = new EntityRepository();
        Updater = gameObject.AddComponent<UpdateManager>();
        ChunkManager = new ChunkManager(chunkSize);
    }
    private void Awake()
    {
        Application.targetFrameRate -= 1;
        Instance = this;
        InitManagers();
        Prefabs.Init();
        poolStorage.InitPools();
        //Application.targetFrameRate = 60;
        LoadResources();
        //TODO перенос в chunk
        //for (int i = 0; i<2; i++)
        //{
        //    var bonfire = Instantiate(bonfirePref, Quaternion.AngleAxis(Random.Range(0,360), Vector3.forward) * new Vector3(1500 + Random.Range(-1000, 1000), 1500 + Random.Range(-1000, 1000)), transform.rotation).GetComponent<Bonfire>();
        //    bonfire.health = 3;
        //}
        AddChunk(new Vector2Int(-1, -1));
        AddChunk(new Vector2Int(0, -1));
        AddChunk(new Vector2Int(1, -1));
        AddChunk(new Vector2Int(-1, 0));
        AddChunk(new Vector2Int(0, 0));
        AddChunk(new Vector2Int(1, 0));
        AddChunk(new Vector2Int(-1, 1));
        AddChunk(new Vector2Int(0, 1));
        AddChunk(new Vector2Int(1, 1));
    }
    private void Start()
    {
        AudioPlayer.Instance.PlayBackgroundMusic(worldTheme, 0.5f);
    }

    private void OnEnable()
    {
        Updater.Add(this);
    }
    private void OnDisable()
    {
        Updater.Remove(this);
    }
    public void ManualUpdate()
    {
        //Смена чанков при передвижении игрока
        if (playerControl != null)
        {
            var playerCoord = Vector2Int.RoundToInt(playerControl._transform.position / chunkSize);
            if (playerCoord != currentChunk)
            {
                var prevChunk = currentChunk;
                currentChunk = chunks[playerCoord].coord;

                //TODO перенести куда-то и учитывать направление перехода
                var direction = (prevChunk - currentChunk);
                for (int i = -1; i <= 1; ++i)
                {
                    var coordX = (direction.x == 0) ? i : direction.x;
                    var coordY = (direction.x == 0) ? direction.y : i;

                    var coord = new Vector2Int(coordX, coordY);
                    //if (bonfires.ContainsKey(prevChunk + coord))
                    //    bonfires[prevChunk + coord].DisableEffects();
                    //if (bonfires.ContainsKey(currentChunk - coord))
                    //    bonfires[currentChunk - coord].EnableEffects();
                }

                chunks[prevChunk].UnMakeCurrent();
                chunks[playerCoord].MakeCurrent();

                stringBuilder.Length = 0;
                stringBuilder.Append("Квадрат: (");
                stringBuilder.Append(currentChunk[0]);
                stringBuilder.Append("; ");
                stringBuilder.Append(currentChunk[1]);
                stringBuilder.Append(")");
                
                coordinatesUI.textMesh.text = stringBuilder.ToString();
            }
        }
    }

    public void DeleteStorages()
    {
        var drops = Entities.DropItems;
        for (int i = drops.Count - 1; i >= 0; --i)
        {
            if (!drops[i].toDestroy && !drops[i].IsQuestItems() && !drops[i].dontDestroy)
            {
                drops[i].toDestroy = true;
            }
            else
            {
                if (!drops[i].IsQuestItems() && !drops[i].dontDestroy)
                {
                    Destroy(drops[i].gameObject);
                }
            }
        }
    }

}
