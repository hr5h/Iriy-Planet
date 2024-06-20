using Game.Sounds;
using System.Collections;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Experimental.Rendering.Universal;

public class Bonfire : MonoBehaviour
{
    private EntityRepository Entities => WorldController.Instance.Entities;

    public float range;
    public float health;
    public bool dontDestroy;
    public CircleCollider2D area;
    public GameObject oldBonfirePref;

    public AudioSource audioSource;
    public AudioClip bonfireEndClip;

    [SerializeField] private Light2D _light;
    [SerializeField] private ParticleSystem _particleSystem;

    private Vector2Int _currentChunk;

    private Vector2Int _coord;
    private void Start()
    {
        _coord = Vector2Int.RoundToInt(transform.position / WorldController.Instance.chunkSize);
        area.radius = range;

        if ((math.abs(WorldController.Instance.currentChunk.x - _coord.x) > 1) || 
            (math.abs(WorldController.Instance.currentChunk.y - _coord.y) > 1))
        {
            DisableEffects();
        }
    }
    public void EnableEffects()
    {
        _light.enabled = true;
        _particleSystem.Play();
    }
    public void DisableEffects()
    {
        _light.enabled = false;
        _particleSystem.Stop();
    }

    public void ReduceHealth() //Затухание костра
    {
        if (!dontDestroy)
        {
            health--;
            if (health <= 0)
            {
                AudioPlayer.Instance.PlaySoundFX(bonfireEndClip, transform.position);
                Instantiate(oldBonfirePref, transform.position, transform.rotation);
                Destroy(gameObject);
            }
        }
    }

    private void OnEnable() => StartCoroutine(OnEnableCoroutine());
    private IEnumerator OnEnableCoroutine() //Так как OnEnable данного класса может вызваться раньше, чем Awake класса WorldController, то ожидаем один кадр
    {
        yield return null;
        _currentChunk = WorldController.Instance.ChunkManager.CalculateCurrentChunk(transform.position);
        WorldController.Instance.ChunkManager.Add(_currentChunk, this);
        Entities.Add(this);
    }
    private void OnDisable()
    {
        WorldController.Instance.ChunkManager.Remove(_currentChunk, this);
        Entities.Remove(this);
    }
}
