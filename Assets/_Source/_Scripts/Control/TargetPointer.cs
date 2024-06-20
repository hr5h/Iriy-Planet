using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

[RequireComponent(typeof(CircleCollider2D)), RequireComponent(typeof(Image))]
public class TargetPointer : MonoBehaviour, IUpdatable
{
    [HideInInspector] public UnityEvent<Damageable> OnJump;
    [HideInInspector] public UnityEvent OnDistanceBroken;
    public float JumpRadius { get => _jumpRadius; private set => _jumpRadius = value; }
    public float AcceptableDistance { get => _acceptableDistance; private set => _acceptableDistance = value; }
    
    [SerializeField] private float _jumpRadius;
    [SerializeField] private float _acceptableDistance;
    
    private Damageable _prevTarget;
    private Damageable _target;
    private Damageable _owner;
    //Приоритет выбора цели: люди > монстры > препятствия
    private readonly List<Human> _possibleHumans = new List<Human>();
    private readonly List<Monster> _possibleMonsters = new List<Monster>();
    private readonly List<Shelter> _possibleShelters = new List<Shelter>();
    private readonly List<Damageable> _possibleTargets = new List<Damageable>();
    
    private CircleCollider2D _collider;
    private Image _image;
    
    private float _timer = 1f;
    private void Start()
    {
        _collider = GetComponent<CircleCollider2D>();
        _collider.enabled = false;
        _image = GetComponent<Image>();
        _owner = WorldController.Instance.playerControl.human;
        _owner.OnDeath.AddListener(OnOwnerDeath);
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
        if (_timer > 0f)
        {
            _timer -= Time.deltaTime;
            if (_timer <= 0f)
            {
                if (_target != null && !IsAcceptableDistance())
                {
                    ResetTarget();
                    OnDistanceBroken?.Invoke();
                }
                _timer = 1f;
            }
        }
        if (_target != null)
        {
            MoveToTarget();
        }
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.TryGetComponent<Damageable>(out var damageable))
        {
            if (damageable != _prevTarget && damageable != _owner && !_possibleTargets.Contains(damageable))
            {
                AddPossibleTarget(damageable);
            }
        }
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.TryGetComponent<Damageable>(out var damageable))
        {
            if (_possibleTargets.Contains(damageable))
            {
                RemovePossibleTarget(damageable);
            }
        }
    }
    private void AddPossibleTarget(Damageable target)
    {
        target.OnDeath.AddListener(OnPossibleTargetDeath);
        _possibleTargets.Add(target);
        switch (target)
        {
            case Human human:
                _possibleHumans.Add(human);
                break;
            case Monster monster:
                _possibleMonsters.Add(monster);
                break;
            case Shelter shelter:
                _possibleShelters.Add(shelter);
                break;
        }
    }
    private void RemovePossibleTarget(Damageable target)
    {
        target.OnDeath.RemoveListener(OnPossibleTargetDeath);
        _possibleTargets.Remove(target);
        switch (target)
        {
            case Human human:
                _possibleHumans.Remove(human);
                break;
            case Monster monster:
                _possibleMonsters.Remove(monster);
                break;
            case Shelter shelter:
                _possibleShelters.Remove(shelter);
                break;
        }
    }
    private void RemoveAllPossibleTargets()
    {
        for (int i = 0; i < _possibleTargets.Count; ++i)
        {
            _possibleTargets[i].OnDeath.RemoveListener(OnPossibleTargetDeath);
        }
        _possibleTargets.Clear();
        _possibleHumans.Clear();
        _possibleMonsters.Clear();
        _possibleShelters.Clear();
    }
    private void MoveToTarget()
    {
        transform.position = _target.transform.position;
    }
    private bool IsAcceptableDistance()
    {
        return Vector2.Distance(_target.transform.position, _owner.transform.position) <= _acceptableDistance;
    }
    private void OnPossibleTargetDeath(Damageable target, Damageable killer = null)
    {
        RemovePossibleTarget(target);
    }
    private void OnTargetDeath(Damageable target = null, Damageable killer = null)
    {
        _prevTarget = _target;
        ResetTarget();
        TryJump();
    }
    public void SetTarget(Damageable target)
    {
        if (_target != null)
        {
            ResetTarget();
        }
        _target = target;
        _target.OnDeath.AddListener(OnTargetDeath);
        
        MoveToTarget();
        Show();
    }
    public void ResetTarget()
    {
        if (_owner == null) return;
        //if (!_target.IsDead)
        //    AddPossibleTarget(_target);
        StartCoroutine(LookPosResetCoroutine());
        _target.OnDeath.RemoveListener(OnTargetDeath);
        _target = null;

        Hide();
    }
    private IEnumerator LookPosResetCoroutine()
    {
        yield return Yielders.Get(0.1f); //Чтобы камера не дергалась, выжидаем время после смены цели
        if (_target == null)
        {
            WorldController.Instance.playerControl.lookPoint.ResetPos();
        }
    }
    public void SetJumpRadius(float radius)
    {
        if (radius > 0f)
        {
            _jumpRadius = radius;
            _collider.radius = _jumpRadius;
        }
    }
    public void TryJump()
    {
        StartCoroutine(JumpCoroutine());
    }
    private IEnumerator JumpCoroutine()
    {
        RemoveAllPossibleTargets();
        _collider.enabled = true;
        
        yield return null; // Это нужно, чтобы  успевали обработаться коллизии после включения коллайдера (плохое решение)
        yield return null;


        if (_possibleTargets.Count == 0)
        {
            _collider.enabled = false;
            yield break;
        }

        Damageable nearestPossibleTarget = null;
        if (_possibleHumans.Count > 0)
        {
            nearestPossibleTarget = _possibleHumans
                            .Where(target => target.relation[(_owner as Human).clan] < 0 || ((target.ai as HumanAI != null) && (target.ai as HumanAI).personalEnemies.Contains(_owner)))
                            .OrderBy(target => Vector2.Distance(transform.position, target.transform.position))
                            .FirstOrDefault();
        }
        if (nearestPossibleTarget == null && _possibleMonsters.Count > 0)
        {
            nearestPossibleTarget = _possibleMonsters
                            .OrderBy(target => Vector2.Distance(transform.position, target.transform.position))
                            .FirstOrDefault();
        }
        if (nearestPossibleTarget == null && (_prevTarget is Shelter) && _possibleShelters.Count > 0)
        {
            nearestPossibleTarget = _possibleShelters
                            .OrderBy(target => Vector2.Distance(transform.position, target.transform.position))
                            .FirstOrDefault();
        }

        if (nearestPossibleTarget != null)
        {
            SetTarget(nearestPossibleTarget);
            OnJump?.Invoke(_target);
        }
        _collider.enabled = false;
    }
    private void OnOwnerDeath(Damageable owner = null, Damageable killer = null)
    {
        StopAllCoroutines();
        _owner = null;
        gameObject.SetActive(false);
    }
    private void Show()
    {
        _image.enabled = true;
    }
    private void Hide()
    {
        _image.enabled = false;
    }
}
