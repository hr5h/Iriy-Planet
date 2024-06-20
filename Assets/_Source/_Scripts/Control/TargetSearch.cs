using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

[RequireComponent(typeof(CircleCollider2D))]
public class TargetSearch : MonoBehaviour
{
    public static TargetSearch instance;
    public Human player;
    public List<Human> enemies;
    public List<Human> allies;
    public List<Shelter> shelters;
    public List<Monster> monsters;
    public Damageable nearestTarget;

    public bool onlyEnemies;
    private float dist;
    private CircleCollider2D _collider;
    public void Search(Vector2 pos, bool _onlyEnemies = false)
    {
        //Logger.Debug("Search");
        onlyEnemies = _onlyEnemies;
        transform.position = pos;
        StopAllCoroutines();
        StartCoroutine(SearchCoroutine());
    }
    private IEnumerator SearchCoroutine()
    {
        ClearAllContainers();

        _collider.enabled = true;
        _collider.radius = 32;

        yield return null; // Это нужно, чтобы  успевали обработаться коллизии после включения коллайдера (плохое решение)
        yield return null;

        FindNearestTarget();

        _collider.enabled = false;
        _collider.radius = 0;
    }
    private void FindNearestTarget()
    {
        nearestTarget = null;
        dist = float.MaxValue;

        if (onlyEnemies)
        {
            foreach (var enemy in enemies)
            {
                var d = Vector2.Distance(enemy.transform.position, transform.position);
                if (d < dist)
                {
                    nearestTarget = enemy;
                    dist = d;
                }
            }
            if (nearestTarget == null)
            {
                foreach (var monster in monsters)
                {
                    var d = Vector2.Distance(monster.transform.position, transform.position);
                    if (d < dist)
                    {
                        nearestTarget = monster;
                        dist = d;
                    }
                }
            }
        }
        else
        {
            foreach (var enemy in enemies)
            {
                var d = Vector2.Distance(enemy.transform.position, transform.position);
                if (d < dist)
                {
                    nearestTarget = enemy;
                    dist = d;
                }
            }
            foreach (var monster in monsters)
            {
                var d = Vector2.Distance(monster.transform.position, transform.position);
                if (d < dist)
                {
                    nearestTarget = monster;
                    dist = d;
                }
            }
            if (nearestTarget == null)
            {
                foreach (var ally in allies)
                {
                    var d = Vector2.Distance(ally.transform.position, transform.position);
                    if (d < dist)
                    {
                        nearestTarget = ally;
                        dist = d;
                    }
                }
            }
            if (nearestTarget == null)
            {
                foreach (var barrier in shelters)
                {
                    var d = Vector2.Distance(barrier.transform.position, transform.position);
                    if (d < dist)
                    {
                        nearestTarget = barrier;
                        dist = d;
                    }
                }
            }
        }
        if (nearestTarget != null)
        {
            if (nearestTarget == WorldController.Instance.playerControl.target)
            {
                WorldController.Instance.playerControl.target = null;
                WorldController.Instance.playerControl.targetPointer.ResetTarget();
                TargetInfo.Instance.ResetTarget();
            }
            else
            {
                WorldController.Instance.playerControl.target = nearestTarget;
                WorldController.Instance.playerControl.targetPointer.SetTarget(nearestTarget);
                TargetInfo.Instance.SetTarget(nearestTarget);
            }
        }
    }
    private void Awake()
    {
        instance = this;
        _collider = GetComponent<CircleCollider2D>();
        _collider.enabled = false;
    }
    private void Start()
    {
        WorldController.Instance.playerControl.targetPointer.OnJump.AddListener(OnPointerJumped);
        WorldController.Instance.playerControl.targetPointer.OnDistanceBroken.AddListener(OnPointerBreakDistance);
    }
    private void OnPointerJumped(Damageable target)
    {
        WorldController.Instance.playerControl.target = target;
        TargetInfo.Instance.SetTarget(target);
    }
    private void OnPointerBreakDistance()
    {
        WorldController.Instance.playerControl.target = null;
        TargetInfo.Instance.ResetTarget();
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.TryGetComponent(out Human human))
        {
            if (human != player)
            {
                if (human.relation[player.clan] < 0 || ((human.ai as HumanAI != null) && (human.ai as HumanAI).personalEnemies.Contains(player)))
                {
                    enemies.Add(human);
                }
                else
                {
                    allies.Add(human);
                }
            }
        }
        if (human == null)
        {
            if (collision.TryGetComponent(out Shelter barrier))
            {
                shelters.Add(barrier);
            }
            if (barrier == null)
            {
                if (collision.TryGetComponent(out Monster monster))
                {
                    monsters.Add(monster);
                }
            }
        }

    }
    private void ClearAllContainers()
    {
        enemies.Clear();
        allies.Clear();
        shelters.Clear();
        monsters.Clear();
    }
}
