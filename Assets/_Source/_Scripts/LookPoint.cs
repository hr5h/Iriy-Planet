using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LookPoint : MonoBehaviour
{
    public float maxDist;
    private Transform _owner;
    private Transform _transform;
    private Vector3 _offset;
    private void Start()
    {
        _transform = GetComponent<Transform>();
        _owner = WorldController.Instance.playerControl.transform;
        WorldController.Instance.playerControl.human.OnDeath.AddListener(OnOwnerDeath);
    }
    private void FixedUpdate()
    {
        _transform.position = _owner.position + _offset;
    }
    private void OnOwnerDeath(Damageable owner = null, Damageable killer = null)
    {
        Destroy(gameObject);
    }
    public void SetPos(Vector3 direction, float dist)
    {
        _offset = Mathf.Min(dist, maxDist) * direction.normalized;
    }
    public void ResetPos()
    {
        _offset = Vector3.zero;
    }
}
