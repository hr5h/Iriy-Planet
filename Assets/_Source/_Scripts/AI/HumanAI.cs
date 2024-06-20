using Game.Sounds;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class HumanAI : AI, IUpdatable
{
    // ТЕСТОВЫЙ ИСКУССТВЕННЫЙ ИНТЕЛЛЕКТ ДЛЯ НПС


    //ОСТАВШИЕСЯ ЗАДАЧИ:

    //TODO Смена отношений между НПС и группировками
    //TODO Перемещение между базами
    //TODO Зачистка территории
    //TODO Поиск артефактов

    public Human owner; //Обладатель ИИ
    public Quaternion look; //Направление взгляда
    public Quaternion lookCurrent; //Текущее направление взгляда
    public bool aimed; //Нацеленный на врага
    public float rotationSpeed; //Скорость поворота
    public float combatInterval; //Интервал между действиями в бою

    public List<Human> personalEnemies = new List<Human>(); //Список личных врагов (независимо от группировки)
    public Queue<Vector3> routeMap = new Queue<Vector3>(); //Карта маршрута
    public List<Vector3> patrolMap = new List<Vector3>(); //Карта патрулирования
    public Vector3 movePosition; // Точка, куда нужно двигаться
    public Vector2 moveVector; //Вектор движения к точке
    public Shelter myShelter; //Укрытие, которое использует НПС
    public Bonfire myBase; //Лагерь, к которому Прикреплен НПС

    private Vector3 _ownerPosition;

    public Queue<Weapon> weaponCollection = new Queue<Weapon>(); //Список оружия, которое есть в зоне видимости
    public Weapon freeWeapon; //Бесхозное оружие, которое идет поднимать НПС

    public Vector2 weaponLength; //Расстояние между НПС и точкой, из которой происходит выстрел

    public bool noWeapon; //Нет оружия
    public bool noAmmo; //Нет патронов
    public bool endlessCombat; //Преследовать врага до конца

    public bool lookAround; //Осматривается
    public Vector2[] lookPoints = new Vector2[3];
    public int lookPointCurrent = 0;
    public bool lookWait;

    public bool escort; //Ведется ли сопровождение
    public GameObject escortTarget; //Объект сопровождения

    public bool route; //Ведется ли следование маршруту
    public Vector3 routePointCurrent; //Текущая точка маршрута
    private bool _hasRoutePoint;

    public bool explore; //Ведется ли осмотр территории
    public bool exploreTime; //Оставшееся время осмотра

    public int patrolPointCurrent;
    public bool patrol;
    public float actionInterval; //Интервал между действиями
    public float lookAroundInterval; //Интервал поворота

    public bool holdPosition; //Удерживается ли позиция
    public Transform Place; //Координаты удерживаемой позиции

    public float strafeTime; //Длительность стрейфа
    public int strafe; //Стрейф
    public bool shotLock; //Запрет на стрельбу
    public float shotInterval; //Интервал при стрельбе очередями
    public float shotTime;

    public bool ignoreBlackout; //Игнорировать затмение

    public GameObject debugCirclePref; //Префаб окружности (для отладки)

    public UnityEvent OnBaseLeave; //Событие выхода НПС из базы
    public UnityEvent RemoveState; //Отмена патруля и прочих состояний

    public bool dontMonsterAttack; //Не атакует монстров
    public bool dontHumanAttack; //Не атакует людей
    public bool dontCombat; //Вообще не реагирует на противников

    public AudioClip stepClip;
    public bool stepBool = true;

    public enum AIState //Состояния персонажа
    {
        Idle, //Бездействие
        Combat, //Бой
        Escape, //Бегство
        FindWeapon, //Поиск оружия
        Task, //Выполнение задания
        ReturnToBase, //Возвращение на базу
        Explore, //Обследование территории
        Patrol, //Патрулирование
        Route, //Следование маршруту
        Escort, //Сопровождение
        HoldPosition, // Удерживание позиции
    }
    public AIState myState;

    public void LeaveBase()
    {
        OnBaseLeave?.Invoke();
    }
    public void Start()
    {
        if (creature)
        {
            owner = creature as Human;
        }
        if (owner.money == 0)
        {
            int koef = Random.Range(5, 20);
            owner.money = koef * 100;
        }
        rigidBody = owner.GetComponent<Rigidbody2D>();
        visionCollider.transform.localScale = Vector3.one * owner.visionRange;
        owner.ai = this;
        owner.TakeAmmo("5.45", 900);
        weaponLength = owner.body.weapon_shoot.localPosition;
        myState = AIState.Idle;
        noAmmo = false;
        shotLock = true;
    }
    //public void DebugDrawCircle(Vector2 pos, Color col)
    //{
    //    var circle = Instantiate(debugCirclePref, pos, transform.rotation).GetComponent<SpriteRenderer>();
    //    circle.color = col;
    //}
    public Bonfire FindNearestCamp() // Поиск ближайшего лагеря
    {
        Bonfire res = null;
        float t = -1;
        var bonfires = owner.worldController.Entities.Bonfires;
        for (int i = bonfires.Count - 1; i >= 0; i--)
        {
            var dist = Vector2.Distance(_ownerPosition, bonfires[i].transform.position);
            if ((t == -1) || (dist < t))
            {
                t = dist;
                res = bonfires[i];
            }
        }

        return res;
    }
    public override void SetTarget(Character other)
    {
        lookAround = false;
        if (target == null)
        {
            if (other == null)
                return;
            if (other is Human)
            {
                var h = other as Human;
                if (owner.relation[h.clan] < 0 || h.mainHero)
                {
                    base.SetTarget(other);
                }
            }
            else
            {
                base.SetTarget(other);
            }
            var vect = other.transform.position - transform.position;
            look = Quaternion.FromToRotation(Vector3.right, vect);
            aimed = false;
        }
        else
        {
            targetContainer.Enqueue(other);
        }
    }
    public void TargetHandler() //Обработка целей
    {
        if (!target)
        {
            FindTarget();
        }
        else
        {
            if (myState == AIState.ReturnToBase)
                move = false;
            if (!noWeapon)
            {
                if (!noAmmo)
                {
                    aimed = false;
                    myState = AIState.Combat;
                    Combat();
                }
                else
                {
                    myState = AIState.Escape;
                    Escape();
                }
            }
            else
            {
                myState = AIState.FindWeapon;
                FindWeapon();
            }
        }
    }
    public void BlackoutHandler() //Обработка затмений
    {
        if ((owner.worldController.blackout) && (!owner.inCamp) && (!target) && (!escort) && (!ignoreBlackout))
        {
            myBase = FindNearestCamp();
            if (myBase != null)
            {
                myState = AIState.ReturnToBase;
                ReturnToBase();
                movePosition = myBase.transform.position - (myBase.transform.position - _ownerPosition).normalized * 64f;
                move = true;
            }
        }
    }
    public void ReturnToBase() //Возвращение на базу
    {
        if (myBase == null)
        {
            myState = AIState.Idle;
            return;
        }
        Vector2 vect = myBase.transform.position - _ownerPosition;
        look = Quaternion.FromToRotation(Vector3.right, vect);
        if (!move)
        {
            myState = AIState.Idle;
            return;
        }
        TargetHandler();
    }

    public void Idle() //Состояние бездействия
    {
        BlackoutHandler();
        TargetHandler();
        if (patrol)
        {
            myState = AIState.Patrol;
        }
        if (route)
        {
            myState = AIState.Route;
            //Фикс того, что если нпс отвлекся от маршрута, возвращается назад к изначальной точке
            if (routePointCurrent != null)
            {
                while (routeMap.Count > 0 && (Vector2.Distance(routePointCurrent, routeMap.Peek()) >= Vector2.Distance(routeMap.Peek(), _ownerPosition)))
                {
                    Logger.Debug("Current route point skipped!");
                    routePointCurrent = routeMap.Dequeue();
                    movePosition = routePointCurrent;
                }
            }
        }
        if (escort)
        {
            myState = AIState.Escort;
        }
        if (holdPosition)
        {
            myState = AIState.HoldPosition;
        }
    }
    public void Patrol() //Состояние патрулирования
    {
        if (!patrol)
        {
            myState = AIState.Idle;
            return;
        }
        BlackoutHandler();
        TargetHandler();
        if (!WorldController.Instance.blackout || ignoreBlackout)
            if (!move && actionInterval == 0)
            {
                LookAtPoint(patrolMap[patrolPointCurrent]);
                movePosition = patrolMap[patrolPointCurrent];
                move = true;
            }
        if (Vector2.Distance(_ownerPosition, patrolMap[patrolPointCurrent]) < 25)
        {
            move = false;
            actionInterval = 3;
            LookAround();
            patrolPointCurrent++;
            if (patrolPointCurrent >= patrolMap.Count)
            {
                patrolPointCurrent = 0;
            }
        }
    }
    public void Route() //Состояние следования маршруту
    {
        if (!route)
        {
            myState = AIState.Idle;
            return;
        }
        BlackoutHandler();
        TargetHandler();
        if (!WorldController.Instance.blackout || ignoreBlackout)
            if (!move && actionInterval == 0 && !lookAround)
            {
                if (_hasRoutePoint == false)
                {
                    _hasRoutePoint = true;
                    if (routeMap.Count > 0)
                    {
                        routePointCurrent = routeMap.Dequeue();
                    }
                }
                LookAtPoint(routePointCurrent);
                movePosition = routePointCurrent;
                move = true;
            }
        if (_hasRoutePoint && Vector2.Distance(_ownerPosition, routePointCurrent) < 25)
        {
            _hasRoutePoint = false;
            move = false;
            actionInterval = 1;
            LookAround();
            if (routeMap.Count == 0)
            {
                route = false;
            }
        }
    }
    public void Escort() //Сопровождение другого объекта
    {
        if (!escort)
        {
            myState = AIState.Idle;
            return;
        }
        //BlackoutHandler();
        TargetHandler();
        if (escortTarget == null) //Если нет цели для следования, сменить состояние НПС
        {
            escort = false;
            return;
        }
        var dist = Vector2.Distance(_ownerPosition, escortTarget.transform.position);
        if (!move && dist > 150)
        {
            LookAtPoint(escortTarget.transform.position);
            movePosition = Vector2.Lerp(_ownerPosition, escortTarget.transform.position, 0.7f);
            move = true;
        }
        if (move && dist < 25)
        {
            move = false;
            actionInterval = 1;
        }
    }
    public void Explore() //Исследование местности
    {
        if (!explore)
        {
            myState = AIState.Idle;
            return;
        }
        BlackoutHandler();
        TargetHandler();
        //TODO Explore();
    }
    public void HoldPosition() //Удерживание позиции
    {
        if (!holdPosition)
        {
            myState = AIState.Idle;
            return;
        }
        BlackoutHandler();
        TargetHandler();
        var dist = Vector2.Distance(_ownerPosition, Place.position);
        if (!move && dist > 100)
        {
            LookAtPoint(Place.position);
            movePosition = Place.position;
            move = true;
        }
        if (dist < 50)
        {
            if (move)
            {
                move = false;
                actionInterval = 1;
                look = Place.rotation;
            }
            else
            {
                if (!lookAround && actionInterval == 0)
                {
                    LookAround();
                    lookAroundInterval = 1;
                    actionInterval = 9;
                }
            }
        }
    }
    private void FixedUpdate()
    {
        if (move)
        {
            if (stepBool)
            {
                AudioPlayer.Instance.PlaySoundFX(stepClip, owner.stepSource);
                stepBool = false;
                StartCoroutine(StepBool());
            }
            owner.swing += owner.swingCoef * owner.movement * Time.fixedDeltaTime;
            rigidBody.AddForce(owner.moveSpeed * Time.fixedDeltaTime * moveVector);
        }
    }
    //private void FixedUpdate()
    //{
    //    if (move)
    //    {
    //        if (stepBool)
    //        {
    //            audioSource.PlayOneShot(stepClip);
    //            stepBool = false;
    //            StartCoroutine(StepBool());
    //        }
    //        owner.swing += owner.swingCoef * owner.movement * Time.fixedDeltaTime;

    //        Vector2 newPosition = (Vector2)_ownerPosition + moveVector * owner.moveSpeed / 40000 * Time.fixedDeltaTime;
    //        rigidBody.MovePosition(newPosition);
    //    }
    //}
    public IEnumerator StepBool()
    {
        yield return Yielders.Get(0.5f);
        stepBool = true;
    }

    public void LookAtPoint(Vector2 point) //Смотреть на точку
    {
        Vector2 vect = point - (Vector2)_ownerPosition;
        look = Quaternion.FromToRotation(Vector3.right, vect);
    }
    public void LookNext()
    {
        if (lookWait)
        {
            lookWait = false;
            lookPointCurrent++;
            if (lookPointCurrent == lookPoints.Length)
            {
                lookAround = false;
                return;
            }
            LookAtPoint(lookPoints[lookPointCurrent]);
        }
    }
    public Quaternion[] angles = new Quaternion[3] 
    { 
        Quaternion.Euler(0, 0, 70),
        Quaternion.Euler(0, 0, -70),
        Quaternion.Euler(0, 0, 0)};
    public void LookAround()
    {
        lookAround = true;
        lookPointCurrent = 0;

        Vector3 right = owner._transform.right;
        float distance = 100;

        for (int i = 0; i < lookPoints.Length; i++)
        {
            lookPoints[i] = _ownerPosition + angles[i] * right * distance;
        }

        LookAtPoint(lookPoints[0]);
    }
    public Collider2D Obstacles(Character other) // Есть ли препятствие между НПС и целью. Если есть, то возвращает коллайдер препятствия
    {
        Vector2 normal = (other._transform.position - _ownerPosition).normalized;
        Vector2 shotPlace = (Vector2)_ownerPosition + normal * weaponLength.x + Vector2.Perpendicular(normal) * weaponLength.y;
        Collider2D res = null;
        var hit = Physics2D.Raycast(shotPlace, ((Vector2)other._transform.position - shotPlace).normalized, owner.visionRange, ~(1 << 3) & ~(1 << 7) & ~(1 << 11) & ~(1 << 12)).collider; //Луч на проверку наличия препятствий между объектом и целью
        if (hit) //Если столкновение есть
        {
            if (hit.gameObject != target.gameObject) //Если луч не дошел до цели
            {
                if (hit.gameObject.TryGetComponent(out Human obj)) //Если это человек
                {
                    if (owner.relation[obj.clan] >= 0) //Если у нас хорошие отношения с его кланом
                    {
                        if (personalEnemies.FindIndex(x => obj) == -1) //Если не является личным врагом
                        {
                            res = hit;
                        }
                    }
                }
                else
                {
                    if (hit.gameObject.TryGetComponent(out Shelter obj2)) //Если это укрытие
                    {
                        res = hit;
                    }
                }
            }
        }
#if UNITY_EDITOR
        if (res)
        {
            Debug.DrawLine(shotPlace, other._transform.position, Color.red);
        }
        else
        {
            Debug.DrawLine(shotPlace, other._transform.position, Color.green);
        }
#endif
        return res;
    }
    public Character ResetTarget() //Сбросить цель
    {
        Character res = target;
        //DebugDrawCircle(target._transform.position, Color.red);
        aimed = false;
        strafe = 0;
        strafeTime = 0;
        shotTime = 0;
        shotLock = true;
        target = null;
        return res;
    }
    public void Combat() //Состояние боя
    {
        if (target != null)
        {
            if (dontCombat)
            {
                target = null;
                myState = AIState.Idle;
                return;
            }
            //TODO Сделать ближний бой при нехватке патронов или при малой дистанции до цели
            //TODO Прекратить бой при слишком большой дистанции до цели
            Vector2 vect = target._transform.position - _ownerPosition;
            look = Quaternion.FromToRotation(Vector3.right, vect);
            var dist = Vector2.Distance(target._transform.position, _ownerPosition);
            if (dist < 150)
            {
                move = true;
                movePosition = _ownerPosition - owner._transform.right * 2;
            }
            if ((dist > owner.visionRange * 2f) && (!endlessCombat))
            {
                target = null;
                myState = AIState.Idle;
                return;
            }
            if (aimed)
            {
                var obstacle = Obstacles(target);
                if (obstacle == null)
                {
                    if (shotTime == 0)
                    {
                        shotTime = shotInterval;
                        shotLock = !shotLock;
                    }
                    if (!shotLock)
                    {
                        //TODO добавление НПС патронов при нехватке. Переработать
                        if ((owner.weapon.ammo == 0) && (owner.Ammo[owner.weapon.data.ammoType] == 0))
                        {
                            owner.TakeAmmo(owner.weapon.data.ammoType, owner.weapon.data.clip);
                        }
                        owner.weapon.Shot();
                    }
                    owner.swing = 0;
                    if (combatInterval == 0)
                    {
                        combatInterval = 0.8f + Random.Range(-0.2f, 0.2f);
                        if (!move)
                        {
                            var r = Random.Range(0, 3);
                            switch (r)
                            {
                                case 0:
                                    {
                                        move = true;
                                        movePosition = _ownerPosition + Random.Range(-1, 2) * Random.Range(75, 135) * owner._transform.up;
                                    }
                                    break;
                                case 1:
                                    {
                                        var rr = Random.Range(0, 2);
                                        switch (rr)
                                        {
                                            case 0: { strafe = 1; } break;
                                            default: { strafe = -1; } break;
                                        }
                                        move = true;
                                        movePosition = transform.position;
                                        strafeTime = Random.Range(1f, 3f);
                                    }
                                    break;
                            }
                        }
                    }
                }
                else
                {
                    if (obstacle.TryGetComponent(out Shelter barrier)) //Если препятствием является укрытие
                    {
                        if (Vector2.Distance(_ownerPosition, barrier.transform.position) < Vector2.Distance(target._transform.position, barrier.transform.position))
                        {
                            myShelter = barrier;
                            move = true;
                            movePosition = _ownerPosition + owner._transform.right * 2;
                            //Если мы ближе у нему, чем враг, то занять позицию возле укрытия
                        }
                        else
                        {
                            //Иначе
                            //1. Остаться на месте

                            //2. Обойти влево или вправо
                            if (combatInterval == 0)
                            {
                                var r = Random.Range(0, 2);
                                {
                                    {
                                        combatInterval = 0.5f;
                                        move = true;
                                        movePosition = _ownerPosition + Random.Range(-1, 2) * 100 * owner._transform.up;
                                    }
                                }
                            }
                            //3. Разрушить укрытие
                            if (target is Human)
                            {
                                //TODO добавление НПС патронов при нехватке. Переработать
                                if ((owner.weapon.ammo == 0) && (owner.Ammo[owner.weapon.data.ammoType] == 0))
                                {
                                    owner.TakeAmmo(owner.weapon.data.ammoType, owner.weapon.data.clip);
                                }
                                owner.weapon.Shot();
                                owner.swing = 0;
                            }
                        }
                    }
                    else
                    {
                        if (combatInterval == 0)
                        {
                            var r = Random.Range(0, 2);
                            {
                                {
                                    combatInterval = 0.5f;
                                    move = true;
                                    movePosition = _ownerPosition + Random.Range(-1, 2) * 100 * owner._transform.up;
                                }
                            }
                        }
                    }
                }
            }
        }
        else
        {
            LookAround();
            myState = AIState.Idle;
        }
    }
    public void Escape() //Состояние бегства
    {
        //TODO Реализовать логику бегства НПС от опасности
    }
    public void FindWeapon() //Состояние поиска оружия
    {
        if (freeWeapon == null)
        {
            while (weaponCollection.Count > 0)
            {
                var temp = weaponCollection.Dequeue();
                if (temp.owner == null)
                {
                    freeWeapon = temp;
                    break;
                }
            }
        }
        else
        {
            move = true;
            movePosition = freeWeapon._transform.position;
            Vector2 vect = movePosition - _ownerPosition;
            look = Quaternion.FromToRotation(Vector3.right, vect);
            //if (look < 0) look += 360;
            if (!noWeapon) myState = AIState.Idle;
            if (freeWeapon.owner != null) freeWeapon = null;
        }
    }
    public void FindTarget() //Поиск цели
    {
        while (targetContainer.Count > 0)
        {
            var temp = targetContainer.Dequeue();
            if (temp != null)
            {
                if (Vector2.Distance(_transform.position, temp._transform.position) <= owner.visionRange + 48f)
                {
                    if (temp is Monster && dontMonsterAttack)
                        break;
                    if (temp is Human && dontHumanAttack)
                        break;
                    SetTarget(temp);
                    //DebugDrawCircle(target._transform.position, Color.yellow);
                    break;
                }
                else
                {
                    //DebugDrawCircle(temp._transform.position, Color.red);
                }
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
        _ownerPosition = owner._transform.position;
        var deltaTime = Time.deltaTime;
        if (shotTime != 0)
        {
            shotTime = Mathf.Max(0, combatInterval - deltaTime);
            shotTime = Mathf.Max(0, combatInterval - deltaTime);
        }
        if (lookAroundInterval != 0)
        {
            lookAroundInterval = Mathf.Max(0, lookAroundInterval - deltaTime);
        }
        if (actionInterval != 0)
        {
            actionInterval = Mathf.Max(0, actionInterval - deltaTime);
        }
        if (combatInterval != 0)
        {
            combatInterval = Mathf.Max(0, combatInterval - deltaTime);
        }
        if (strafeTime != 0)
        {
            strafeTime = Mathf.Max(0, strafeTime - deltaTime);
            if (strafeTime == 0)
            {
                strafe = 0;
                LookAround();
            }
        }
        if (move)
        {
            if (strafe == 0)
            {
                moveVector = (movePosition - _ownerPosition).normalized;
                //owner.transform.rotation *= Quaternion.Euler(0, 0, owner.swingCoef);
                if (Vector2.Distance(_transform.position, movePosition) < 25)
                {
                    move = false;
                }
            }
            else
            {
                moveVector = strafe * owner._transform.up;
            }
        }
        Quaternion swing = Quaternion.Euler(0, 0, owner.swing);
        var angle = (Quaternion.Angle(owner.transform.rotation, look * swing));
        if (Mathf.Abs(angle) >= 5)
        {
            aimed = false;
            owner.transform.rotation = Quaternion.RotateTowards(owner.transform.rotation, look, rotationSpeed * deltaTime);
        }
        else
        {
            owner.transform.rotation = look * swing;
            if (lookAround)
            {
                if (!lookWait)
                {
                    lookWait = true;
                    lookAroundInterval = 1.5f;
                    Invoke(nameof(LookNext), 0.4f + Random.Range(0.1f, 0.3f));
                }
            }
            if (myState == AIState.Combat)
            {
                aimed = true;
            }
        }
        switch (myState)
        {
            case AIState.Idle: Idle(); break;
            case AIState.Combat: Combat(); break;
            case AIState.Escape: Escape(); break;
            case AIState.Escort: Escort(); break;
            case AIState.HoldPosition: HoldPosition(); break;
            case AIState.FindWeapon: FindWeapon(); break;
            case AIState.ReturnToBase: ReturnToBase(); break;
            case AIState.Patrol: Patrol(); break;
            case AIState.Route: Route(); break;
        }
        _transform.position = _ownerPosition;
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.TryGetComponent(out Human human))
        {
            if (human == target) return;
            if (dontHumanAttack)
                return;
            if ((owner.relation[human.clan] < 0) || (personalEnemies.Contains(human)))
            {
                if (target == null)
                {
                    SetTarget(human);
                    //DebugDrawCircle(target._transform.position, Color.yellow);
                }
                else
                {
                    if ((Obstacles(target) != null) && (Obstacles(human) == null))
                    {
                        targetContainer.Enqueue(ResetTarget());
                        SetTarget(human);
                        //DebugDrawCircle(target._transform.position, Color.yellow);
                    }
                    else
                    {
                        SetTarget(human);
                        //DebugDrawCircle(human._transform.position, Color.green);
                    }
                }

            }
            return;
        }
        if (collision.gameObject.TryGetComponent(out Monster monster))
        {
            if (monster == target) return;
            if (dontMonsterAttack)
                return;
            if (target == null)
            {
                target = monster;
                //DebugDrawCircle(target._transform.position, Color.yellow);
            }
            else
            {
                if ((Obstacles(target) != null) && (Obstacles(monster) == null))
                {
                    //print(Obstacles(target));
                    targetContainer.Enqueue(ResetTarget());
                    target = monster;
                    //DebugDrawCircle(target._transform.position, Color.yellow);
                }
                else
                {
                    targetContainer.Enqueue(monster);
                    //DebugDrawCircle(monster._transform.position, Color.green);
                }
            }
            return;
        }
        if (collision.gameObject.TryGetComponent(out Weapon weapon))
        {
            if (weapon.owner == null)
            {
                weaponCollection.Enqueue(weapon);
            }
            return;
        }
    }
}
