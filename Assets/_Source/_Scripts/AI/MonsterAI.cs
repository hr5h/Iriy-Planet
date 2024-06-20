using System.Collections;
using UnityEngine;
//Стандартный искусственный интелект для монстра ближнего боя
public class MonsterAI : AI, IUpdatable
{
    public Monster monster;
    public Vector3 movePosition; // Точка, куда нужно двигаться

    public float walkTimeOut; //Время ожидания перемещения
    public float walkTimeCurrent; //Текущее время ожидания перемещения

    public bool hunting; //Идет ли охота на врага
    public bool isAgressive; //Проявляет ли монстр агрессию
    public bool endlessHunting; //Бесконечная охота (Продолжается ли преследование цели, если она слишком далеко)

    public Animator animator; //Управление анимацией

    public AudioSource audioSource;
    public bool moveBool = true;
    private Vector3 _monsterPos;

    public void Start()
    {
        if (creature)
        {
            monster = creature as Monster;
        }
        audioSource = GetComponent<AudioSource>();
        rigidBody = monster.GetComponent<Rigidbody2D>();
        animator = monster.GetComponent<Animator>();
        animator.speed = 0;
        visionCollider.transform.localScale = Vector3.one * monster.visionRange;
        monster.ai = this;
        walkTimeCurrent = Random.Range(1, walkTimeOut);
    }
    private void FixedUpdate()
    {
        MovementLogic();

    }

    public IEnumerator StepBool()
    {
        yield return Yielders.Get(2f);
        moveBool = true;
    }

    //public void MovementLogic()
    //{
    //    if (monster != null)
    //    {
    //        if (move)
    //        {
    //            rigidBody.AddForce(monster.moveSpeed * Time.fixedDeltaTime * monster._transform.right);
    //            monster.swing += monster.swingCoef * Time.fixedDeltaTime * monster.movement;
    //            if (moveBool)
    //            {
    //                //audioSource.PlayOneShot(monster.moveClip);
    //                moveBool = false;
    //                StartCoroutine(StepBool());
    //            }
    //        }
    //    }
    //}

    public void MovementLogic()
    {
        //if (monster != null) // Просто глупейшая проверка, съедающая ресурсы
        {
            if (move)
            {
                Vector3 newPosition = monster._transform.position + monster._transform.right * monster.moveSpeed / 5000 * Time.fixedDeltaTime;
                rigidBody.MovePosition(newPosition);
                monster.swing += monster.swingCoef * Time.fixedDeltaTime * monster.movement;

                if (moveBool)
                {
                    //audioSource.PlayOneShot(monster.moveClip);
                    moveBool = false;
                    StartCoroutine(StepBool());
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
        _monsterPos = monster._transform.position;
        if (move)
        {
            Vector2 vect = movePosition - _monsterPos;
            direction = Mathf.Atan2(vect.x, vect.y) * Mathf.Rad2Deg;
            monster._transform.eulerAngles = new Vector3(0, 0, 90 - direction + monster.swing);
            if (Vector2.Distance(_monsterPos, movePosition) < 25)
            {
                animator.speed = 0;
                move = false;
            }
        }
        if (target != null)
        {
            Hunt();
        }
        else
        {
            if (hunting)
            {
                hunting = false; //Охота за целью завершена
                animator.speed = 0;
                move = false;
            }
            else
            {
                Idle();
            }
            if (isAgressive)
            {
                FindTarget();
            }
        }
        _transform.position = _monsterPos;
    }
    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Human"))
        {
            if ((target == null) && (isAgressive))
            {
                target = collision.GetComponent<Human>();
            }
            else
            {
                targetContainer.Enqueue(collision.GetComponent<Human>());
            }
        }
    }
    public void Idle()
    {
        if (walkTimeCurrent > 0)
        {
            walkTimeCurrent = Mathf.Max(0, walkTimeCurrent - Time.deltaTime);
            if (walkTimeCurrent == 0)
            {
                walkTimeCurrent = Random.Range(5, walkTimeOut);
                animator.speed = 1;
                move = true;
                movePosition = (Vector2)_monsterPos + new Vector2(Random.Range(-monster.visionRange, monster.visionRange), Random.Range(-monster.visionRange, monster.visionRange));
            }
        }
    }
    public void FindTarget()
    {
        while ((targetContainer.Count > 0) && (target == null))
        {
            var obj = targetContainer.Dequeue();
            if ((obj != null) && (Vector2.Distance(_monsterPos, obj._transform.position) < monster.visionRange))
            {
                target = obj;
            }
        }
    }
    public virtual void Hunt() //Охота
    {
        if (!monster.reload)
        {
            hunting = true;
            float dist = Vector2.Distance(_monsterPos, target._transform.position);
            if (dist < monster.range)
            {
                animator.speed = 0;
                move = false;
                monster.reload = true;
                monster.rateCurrent = monster.rate;
                if ((target.ai != null) && (target.ai.target == null)) target.ai.target = monster;
                monster.Attack(target);
            }
            else
            {
                if (!endlessHunting && (dist > monster.visionRange * 2f))
                {
                    target = null;
                }
                else
                {
                    move = true;
                    animator.speed = 1;
                    movePosition = target._transform.position;
                }
            }
        }
    }

}
