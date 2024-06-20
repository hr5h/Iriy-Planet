using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BloodPool : MonoBehaviour, IUpdatable
{
    public List<Sprite> sprites = new List<Sprite>();
    private SpriteRenderer spriteRenderer;
    [SerializeField] private float startTime;
    private float _lifeTime;
    private Color _color;
    public void Init(Vector2 position, Color color)
    {
        _lifeTime = startTime;
        transform.position = position;
        _color = color;
        spriteRenderer.color = color;
        spriteRenderer.sprite = sprites[Random.Range(0, sprites.Count)];
        transform.eulerAngles = new Vector3(0, 0, Random.Range(0, 360));
    }

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
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
        if (_lifeTime > 0)
        {
            _lifeTime -= Time.deltaTime;
            _color.a -= Time.deltaTime / startTime;
            spriteRenderer.color = _color;
        }
        else
        {
            WorldController.Instance.poolStorage.bloodPoolData.pool.Return(this);
        }
    }
}