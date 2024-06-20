using Game.Sounds;
using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.PlayerLoop;

public class Shelter : Damageable, IUpdatable
{
    private EntityRepository Entities => WorldController.Instance.Entities;

    public SpriteRenderer sprite;
    public Collider2D colliderObject;
    //AudioSource 
    public AudioSource audioSource;
    //AudioClip
    public AudioClip takeDamageClip;
    public AudioClip destroyShelterClip;
    public Material destructionMaterial;
    Material _material;
    private float _scaleFactor = 0f;
    private float _destructionFactor = 1f;
    private bool firstDamage = true;

    public void Awake()
    {
        sprite = GetComponent<SpriteRenderer>();
        //_material = sprite.material;
        //_material.SetFloat("_DestructionAmount", 0);
        colliderObject = GetComponent<Collider2D>();
    }

    public override void TakeDamage(float damage, Damageable damager = null, DamageType damageType = DamageType.Default)
    {
        AudioPlayer.Instance.PlaySoundFXWithRandomPitch(takeDamageClip, audioSource);
        if (firstDamage)
        {
            sprite.sharedMaterial = destructionMaterial;
            _material = sprite.material;
            firstDamage = false;
        }
        base.TakeDamage(damage, damager);
        if (!IsDead)
        {
            _material.SetFloat(ShaderParams.destructionAmount, (1 - (_health / _maxHealth)/2)/1.5f);
        }
    }

    public override void Death(Damageable killer = null)
    {
        AudioPlayer.Instance.PlaySoundFX(destroyShelterClip, audioSource);
        sprite.color = new Color(.9f, .9f, .9f, .9f);
        colliderObject.enabled = false;
        StartCoroutine(DestructionCoroutine());
        base.Death(killer);
    }

    private IEnumerator DestructionCoroutine()
    {
        float amount = _material.GetFloat(ShaderParams.destructionAmount);
        while (amount <= 1)
        {
            _material.SetFloat(ShaderParams.destructionAmount, amount);
            transform.localScale += _scaleFactor * Time.deltaTime * Vector3.one;
            amount += _destructionFactor * Time.deltaTime;
            yield return null;
        }
        Destroy(gameObject);
    }

    public Color particleColor;
    public void ManualUpdate()
    {
        if (Time.frameCount % 60 == 0)
        {
            UpdateCurrentChunk();
        }
    }
    private void UpdateCurrentChunk()
    {
        var coord = Chunks.CalculateCurrentChunk(transform.position);
        if (coord != currentChunk)
        {
            Chunks.ChangeChunk(ref currentChunk, ref coord, this);
        }
    }
    private void OnEnable() => StartCoroutine(OnEnableCoroutine());
    private IEnumerator OnEnableCoroutine() //Так как OnEnable данного класса может вызваться раньше, чем Awake класса WorldController, то ожидаем один кадр
    {
        yield return null;
        Entities.Add(this);
        currentChunk = Chunks.CalculateCurrentChunk(transform.position);
        Chunks.Add(currentChunk, this);
        WorldController.Instance.Updater.Add(this);
    }
    private void OnDisable()
    {
        Entities.Remove(this);
        Chunks.Remove(currentChunk, this);
        WorldController.Instance.Updater.Remove(this);
    }
}
