using System;
using System.Collections;
using UnityEngine;

public class BulletTrail : MonoBehaviour, IUpdatable
{
    private TrailRenderer _rend;
    public Bullet bullet;
    [SerializeField] private float _timeToDestroy;

    public void OnBulletDestroy(Vector2 pos)
    {
        //_rend.emitting = false;
        _timeToDestroy = _rend.time;
        transform.position = pos;
        bullet = null;
    }
    private void Awake()
    {
        _rend = GetComponent<TrailRenderer>();
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
        if (bullet != null)
        {
            transform.position = bullet.transform.position;
        }
        else
        {
            if (_timeToDestroy <= 0)
            {
                WorldController.Instance.poolStorage.bulletTrailData.pool.Return(this);
                _timeToDestroy = 0;
            }
            else
            {
                _timeToDestroy -= Time.deltaTime;
            }
        }
    }

    public void Init(Vector3 position, Quaternion rotation, Color color, float width, float time)
    {
        //_rend.emitting = true;
        _rend.AddPosition(position);
        transform.position = position;
        _rend.endColor = color;
        _rend.startColor = color;
        _rend.startWidth = width;
        _rend.endWidth = width;
        _rend.time = time;
    }
}
