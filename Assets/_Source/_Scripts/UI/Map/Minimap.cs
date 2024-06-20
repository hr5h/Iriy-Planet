using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Minimap : MonoBehaviour, IUpdatable
{
    public MinimapPoint pointPrefab;
    private ChunkManager Chunks => WorldController.Instance.ChunkManager;
    private Vector2Int CurrentChunk => WorldController.Instance.currentChunk;
    private Transform player;
    private float ChunkSize => WorldController.Instance.chunkSize;

    [SerializeField] private float _scaleFactor = 1.5f;
    private float _mapScale;
    private float _imageSize;
    private Image _image;

    [SerializeField] private Image _grid;
    private float _gridSize;

    private Vector2Int[] chunkOffsets = new Vector2Int[]
    {
        new Vector2Int(-1,1),
        new Vector2Int(0,1),
        new Vector2Int(1,1),
        new Vector2Int(-1,0),
        new Vector2Int(0,0),
        new Vector2Int(1,0),
        new Vector2Int(-1,-1),
        new Vector2Int(0,-1),
        new Vector2Int(1,-1),
    };

    private List<MinimapPoint> humanPoints = new List<MinimapPoint>();
    private List<MinimapPoint> bonfirePoints = new List<MinimapPoint>();
    private List<MinimapPoint> labelPoints = new List<MinimapPoint>();

    [SerializeField] private int bonfirePreloadCount = 2;
    [SerializeField] private int labelPreloadCount = 3;
    [SerializeField] private int humanPreloadCount = 15;

    private int _humanCountPrev;
    private int _bonfireCountPrev;
    private int _labelCountPrev;

    private Func<Human, bool> predicateHuman =
        human => !human.mainHero;
    private Func<Bonfire, bool> predicateBonfire =
        bonfire => true;
    private Func<MapLabel, bool> predicateLabel =
        label => label.showOnMinimap;

    private Action<Human, MinimapPoint, Vector2> drawHuman =
        (human, point, position) => point.Draw(position, Color.cyan);
    private Action<Bonfire, MinimapPoint, Vector2> drawBonfire =
        (bonfire, point, position) => point.Draw(position, Color.yellow);
    private Action<MapLabel, MinimapPoint, Vector2> drawLabel =
        (label, point, position) => point.Draw(position, label.color, label.size, label.sprite);

    private void Start()
    {
        player = WorldController.Instance.playerControl.transform;

        PreloadPoints(labelPoints, labelPreloadCount);
        PreloadPoints(bonfirePoints, bonfirePreloadCount);
        PreloadPoints(humanPoints, humanPreloadCount);

        _image = GetComponent<Image>();
        _imageSize = _image.rectTransform.sizeDelta.x;
        _mapScale = _imageSize / (_scaleFactor * ChunkSize);

        _grid.rectTransform.sizeDelta = _mapScale * ChunkSize * 3 * Vector2.one; // Число 3 это количество ячеек в строке в спрайте сетки
        _grid.materialForRendering.SetVector(ShaderParams.offset, Vector2.zero);
        _gridSize = _grid.rectTransform.sizeDelta.x;
    }
    private void PreloadPoints(List<MinimapPoint> container, int preloadCount)
    {
        for (int i = 0; i < preloadCount; ++i)
        {
            var point = Instantiate(pointPrefab, transform);
            point.Clear();
            container.Add(point);
        }
    }
    public void ManualUpdate()
    {
        if (Time.frameCount % 5 == 0)
        {
            int humanCount = 0;
            int bonfireCount = 0;
            int labelCount = 0;

            var position = player.position;

            for (int i = 0; i < chunkOffsets.Length; ++i)
            {
                var coord = CurrentChunk + chunkOffsets[i];
                if (Chunks.Humans.ContainsKey(coord))
                    DrawPoints(Chunks.Humans[coord], humanPoints, position, predicateHuman, drawHuman, ref humanCount);
                if (Chunks.Bonfires.ContainsKey(coord))
                    DrawPoints(Chunks.Bonfires[coord], bonfirePoints, position, predicateBonfire, drawBonfire, ref bonfireCount);
                if (Chunks.MapLabels.ContainsKey(coord))
                    DrawPoints(Chunks.MapLabels[coord], labelPoints, position, predicateLabel, drawLabel, ref labelCount);
            }

            for (int i = humanCount; i < _humanCountPrev; ++i)
                humanPoints[i].Clear();
            for (int i = bonfireCount; i < _bonfireCountPrev; ++i)
                bonfirePoints[i].Clear();
            for (int i = labelCount; i < _labelCountPrev; ++i)
                labelPoints[i].Clear();

            _humanCountPrev = humanCount;
            _bonfireCountPrev = bonfireCount;
            _labelCountPrev = labelCount;

            _grid.materialForRendering.SetVector(ShaderParams.offset, new Vector2(_mapScale * position.x / _gridSize, _mapScale * position.y / _gridSize));
        }
    }

    private void DrawPoints<T>(HashSet<T> layer, List<MinimapPoint> points, Vector3 position, Func<T, bool> predicate, Action<T, MinimapPoint, Vector2> draw, ref int count) where T : MonoBehaviour
    {
        foreach (var item in layer)
        {
            if (predicate(item))
            {
                if (points.Count == count)
                    points.Add(Instantiate(pointPrefab));
                draw(item, points[count], _mapScale * (item.transform.position - position));
                ++count;
            }
        }
    }

    private void OnEnable() => StartCoroutine(OnEnableCoroutine());
    private IEnumerator OnEnableCoroutine() //Так как OnEnable данного класса может вызваться раньше, чем Awake класса WorldController, то ожидаем один кадр
    {
        yield return null;
        WorldController.Instance.Updater.Add(this);
    }
    private void OnDisable()
    {
        WorldController.Instance.Updater.Remove(this);
    }
}
