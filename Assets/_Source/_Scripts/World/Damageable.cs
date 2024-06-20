using UnityEngine;
using UnityEngine.Events;

public abstract class Damageable : MonoBehaviour
{
    protected ChunkManager Chunks => WorldController.Instance.ChunkManager;
    protected Vector2Int currentChunk;

    [HideInInspector] public UnityEvent<Damageable, Damageable> OnDeath;
    [HideInInspector] public UnityEvent<Damageable, Damageable> OnDamage;
    [HideInInspector] public UnityEvent OnHealthChanged;
    public enum DamageType
    {
        Default = 0,
        Pure,
        /*
        Kinetic,
        Chemical,
        Fire,
        Freeze,
        Poison,
        Energy
         */
    }

    [SerializeField] protected string _myName;
    [SerializeField] protected float _health;
    [SerializeField] protected float _maxHealth;
    [SerializeField] protected bool _isDead;
    public string MyName { get => _myName; protected set => _myName = value; }
    public float Health { get => _health; protected set => _health = value; }
    public float MaxHealth { get => _maxHealth; protected set => _maxHealth = value; }
    public bool IsDead { get => _isDead; protected set => _isDead = value; }
    //public virtual void Awake()
    //{
    //    RestoreHealth();
    //}
    public void RestoreHealth()
    {
        _health = _maxHealth;
    }
    protected void SetMaxHealth(float newHealth)
    {
        var hp = newHealth - _maxHealth;
        _maxHealth += hp;
        TakeHeal(hp);
        if (_health <= 0f)
        {
            Death();
        }
    }
    public virtual void TakeDamage(float damage, Damageable damager = null, DamageType damageType = DamageType.Default)
    {
        _health -= damage;
        OnDamage?.Invoke(this, damager);
        if (damage == 0f)
            return;
        OnHealthChanged?.Invoke();
        if (_health <= 0f && !_isDead)
        {
            Death(damager);
        }
    }
    public virtual void TakeHeal(float heal, Damageable healer = null)
    {
        if (_health == _maxHealth)
            return;
        _health = Mathf.Min(_maxHealth, _health + heal);
        OnHealthChanged?.Invoke();
    }
    public virtual void Death(Damageable killer = null)
    {
        _isDead = true;
        OnDeath?.Invoke(this, killer);
    }
}
