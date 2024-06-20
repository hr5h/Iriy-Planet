using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

public class Bullet : MonoBehaviour, IUpdatable
{
    public Human owner; //Владалец оружия, совершившего выстрел
    public WeaponData data;
    private BulletTrail _trail;
    public float angle; //Направление полёта
    public float damage; //Урон
    public float range; //Дальность полёта
    public float speed; //Скорость полёта
    public float reduceCoef; //Коэффициент понижения урона с расстоянием
    public bool isActive { get; private set; }
    [HideInInspector] public Rigidbody2D rigidBody;
    [HideInInspector] public TrailRenderer trailRenderer;
    [HideInInspector] public UnityEvent<Vector2> Created;
    [HideInInspector] public UnityEvent<Vector2> Destroyed;
    public void Start()
    {
        rigidBody = GetComponent<Rigidbody2D>();
    }

    public void Init(WeaponData weaponData)
    {
        data = weaponData;
        damage = data.damage * owner.damageCoef;
        if (data.IsShotgun)
        {
            speed = data.bulletSpeed - UnityEngine.Random.Range(data.bulletSpeed / 10, data.bulletSpeed / 2);
            range = data.range + UnityEngine.Random.Range(-data.range / 10, data.range / 10);
        }
        else
        {
            range = data.range;
            speed = data.bulletSpeed;
        }
        angle = transform.eulerAngles.z;
        rigidBody.velocity = transform.right * speed;
        if (data.damageReduce)
        {
            reduceCoef = data.damage * speed / range * 0.75f; //Формула понижения урона с дальностью
        }
    }

    public void Deactivate(Vector2 pos)
    {
        if (isActive)
        {
            isActive = false;
            WorldController.Instance.poolStorage.bulletData.pool.Return(this);
            Destroyed?.Invoke(pos);
            if (_trail != null)
            {
                _trail.OnBulletDestroy(pos);
                _trail = null;
            }
        }
    }

    private void OnCollisionEnter2D(Collision2D collision) //Столкновение с объектом
    {
        var contactPoint = collision.GetContact(0).point;
        if (collision.gameObject.TryGetComponent(out Character obj))
        {
            if (!obj.IsDead) //TODO баг со смещением траектории пули при столкновении с уже уничтоженным объектом
            {
                //TODO Возвращение владельцу информации об убийстве противника (для изменения статистики и отношений между персонажами)
                WorldController.Instance.SpawnBloodSplash(contactPoint, obj.bloodColor);
                obj.TakeDamage(damage, owner);
                if ((obj.ai != null) && (owner != null))
                {
                    obj.ai.SetTarget(owner);
                }
                Deactivate(contactPoint);

                //Logger.Debug(damage);
            }
        }
        else
        {
            if (collision.gameObject.TryGetComponent(out Shelter obj2))
            {
                WorldController.Instance.SpawnBarrierSplash(contactPoint, obj2.particleColor);
                obj2.TakeDamage(damage);
                Deactivate(contactPoint);
                //Logger.Debug(damage);
            }
        }
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
        range -= speed * Time.deltaTime;
        if (data.damageReduce) //Снижение урона с расстоянием
        {
            damage -= reduceCoef * Time.deltaTime;
        }
        if (range <= 0)
        {
            Deactivate(transform.position);
        }
    }
    public void Init(Vector3 position, Quaternion rotation, Color color, float width, float time, float mass)
    {
        isActive = true;
        transform.SetPositionAndRotation(position, rotation);
        var trail = WorldController.Instance.poolStorage.bulletTrailData.pool.Get();
        trail.Init(position, rotation, color, width, time);
        _trail = trail;
        _trail.bullet = this;
        rigidBody.mass = mass;
        Created?.Invoke(position);
    }
}