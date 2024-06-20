using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

/// <summary>
/// Интерактивная панель, отображающая карту игрового мира
/// </summary>
public class MapDisplay : MonoBehaviour, IDragHandler, IBeginDragHandler, IUpdatable
{
    const int SEGMENT_COUNT = 81;
    private const int UPDATE_FRAME_INTERVAL = 30;

    public MapSegment mapSegmentPrefab;
    public MapPoint mapPointPrefab;

    private ChunkManager Chunks => WorldController.Instance.ChunkManager;

    private MapSegment[] _segments = new MapSegment[SEGMENT_COUNT];

    private List<MapPoint> _points = new List<MapPoint>();
    private int _pointCount;
    [SerializeField] private int _preloadPointCount = 5;

    private Image _image;
    private float _imageSize;
    private float _chunkSize;

    private float _mapScale;

    private int _gridSize;

    private Vector2 _dragPos;
    private Vector2 _offset;

    private float _dragX;
    private float _dragY;

    private float _screenScale;
    private float _cellSize;
    private float _scaledCellSize;

    public void OnScreenResolutionChanged()
    {
        _screenScale = _image.rectTransform.lossyScale.x;
        _scaledCellSize = _cellSize * _screenScale;
    }
    public void OnBeginDrag(PointerEventData eventData)
    {
        _dragPos = eventData.position;
    }

    public void OnDrag(PointerEventData eventData)
    {
        var delta = (eventData.position - _dragPos);
        var gridDelta = delta;

        _offset += delta / _screenScale;

        _dragX += gridDelta.x;
        _dragY += gridDelta.y;

        var scaledVectorX = new Vector2(_scaledCellSize, 0);
        var scaledVectorY = new Vector2(0, _scaledCellSize);

        var shift = Vector2Int.zero;
        while (Mathf.Abs(_dragX) >= _scaledCellSize || Mathf.Abs(_dragY) >= _scaledCellSize)
        {
            if (_dragX >= _scaledCellSize || _dragX <= -_scaledCellSize)
            {
                var deltaX = (_dragX >= _scaledCellSize) ? -scaledVectorX : scaledVectorX;
                gridDelta += deltaX;
                _dragX += deltaX.x;
                shift.x += (int)Mathf.Sign(deltaX.x);
            }

            if (_dragY >= _scaledCellSize || _dragY <= -_scaledCellSize)
            {
                var deltaY = (_dragY >= _scaledCellSize) ? -scaledVectorY : scaledVectorY;
                gridDelta += deltaY;
                _dragY += deltaY.y;
                shift.y += (int)Mathf.Sign(deltaY.y);
            }
        }

        if (shift != Vector2Int.zero)
        {
            for (int i = 0; i < _segments.Length; ++i)
            {
                _segments[i].coord += shift;
                _segments[i].UpdateColor();
            }
            RefreshDisplay();
        }
        else
        {
            for (int i = 0; i < _pointCount; ++i)
            {
                _points[i].transform.Translate(delta);
            }
        }

        for (int i = 0; i < _segments.Length; ++i)
        {
            _segments[i].transform.Translate(gridDelta);
        }
        _dragPos = eventData.position;
    }

    private void Start()
    {
        InitVariables();
        InitMapSegments();
        InitMapPoints();
    }

    #region Initialization methods
    private void InitVariables()
    {
        _chunkSize = WorldController.Instance.chunkSize;
        _image = GetComponent<Image>();
        _imageSize = _image.rectTransform.sizeDelta.x;

        _gridSize = (int)Mathf.Sqrt(SEGMENT_COUNT);

        _cellSize = _imageSize / (_gridSize - 2);
        _mapScale = _cellSize / _chunkSize;
        mapSegmentPrefab.GetComponent<Image>().rectTransform.sizeDelta = new Vector2(_cellSize - 4, _cellSize - 4);
    }
    private void InitMapSegments()
    {
        for (int row = 0; row < _gridSize; ++row)
        {
            for (int col = 0; col < _gridSize; ++col)
            {
                MapSegment segment = Instantiate(mapSegmentPrefab, transform);
                segment.coord = new Vector2Int(col - _gridSize / 2, row - _gridSize / 2);
                segment.UpdateColor();
                segment.transform.localPosition = new Vector3((col - 0.5f) * _cellSize - _imageSize / 2f, (row - 0.5f) * _cellSize - _imageSize / 2f, 0f);
                _segments[row * _gridSize + col] = segment;
            }
        }
        _screenScale = _image.rectTransform.lossyScale.x;
        _scaledCellSize = _cellSize * _screenScale;
    }
    private void InitMapPoints()
    {
        for (int i = 0; i < _preloadPointCount; ++i)
        {
            MapPoint point = Instantiate(mapPointPrefab, transform);
            point.Clear();
            _points.Add(point);
        }
    }
    #endregion

    public void ManualUpdate()
    {
        if (Time.frameCount % UPDATE_FRAME_INTERVAL == 0)
        {
            RefreshDisplay();
        }
    }

    private void RefreshDisplay()
    {
        ClearPoints();

        var pos = _mapScale * (Vector2)(WorldController.Instance.playerControl.transform.position);

        for (int i = 0; i < SEGMENT_COUNT; ++i)
        {
            _segments[i].UpdateColor();
            if (Chunks.MapLabels.ContainsKey(_segments[i].coord))
            {
                foreach (var label in Chunks.MapLabels[_segments[i].coord])
                {
                    if (label.showOnMap && (label.alwaysShow || _segments[i].IsOpen))
                    {
                        GetPoint().Draw(_offset + _mapScale * (Vector2)(label.transform.position), label.color, label.size, label.sprite);
                    }
                }
            }
            if (_segments[i].IsOpen)
            {
                if (Chunks.Bonfires.ContainsKey(_segments[i].coord))
                {
                    foreach (var bonfire in Chunks.Bonfires[_segments[i].coord])
                    {
                        GetPoint().Draw(_offset + _mapScale * (Vector2)(bonfire.transform.position), Color.yellow, 32);
                    }
                }
            }
        }

        GetPoint().Draw(_offset + pos, Color.white, 24);
    }

    private MapPoint GetPoint()
    {
        if (_points.Count == _pointCount)
            _points.Add(Instantiate(mapPointPrefab, transform));
        return _points[_pointCount++];
    }
    private void ClearPoints()
    {
        for (int i = 0; i < _pointCount; ++i)
        {
            _points[i].Clear();
            _points[i].SetSprite(null);
        }
        _pointCount = 0;
    }
    private void OnEnable() => StartCoroutine(OnEnableCoroutine());
    private IEnumerator OnEnableCoroutine() //Так как OnEnable данного класса может вызваться раньше, чем Awake класса WorldController, то ожидаем один кадр
    {
        yield return null;
        RefreshDisplay();
        WorldController.Instance.Updater.Add(this);
    }
    private void OnDisable()
    {
        WorldController.Instance.Updater.Remove(this);
    }
}
