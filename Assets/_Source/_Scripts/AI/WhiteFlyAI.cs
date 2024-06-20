using Game.Sounds;
using System.Collections;
using UnityEngine;

public class WhiteFlyAI : AI, IUpdatable
{
    public Vector3 movePosition; // Точка, куда нужно двигаться
    public Quaternion moveDir; //Направление движения
    public float turnSpeed;

    public float walkTimeOut; //Время ожидания перемещения
    public float walkTimeCurrent; //Текущее время ожидания перемещения

    public bool hunting; //Идет ли охота на врага

    public Animator animator; //Управление анимацией
    public BulletSensor sensor;

    public AudioSource audioSource;
    public AudioClip flyClip;
    public bool flyBool = true;

    public void Start()
    {
        sensor.BulletDetected.AddListener(OnBulletDetected);
        rigidBody = creature.GetComponent<Rigidbody2D>();
        animator = creature.GetComponent<Animator>();
        animator.speed = 0;
        creature.ai = this;
        visionCollider.radius = creature.visionRange;
        walkTimeCurrent = Random.Range(1, walkTimeOut);
    }
    private void OnBulletDetected(Bullet bullet, Collider2D collid)
    {
        Vector2 A = (creature.transform.position - bullet.transform.rotation * Vector3.right);
        Vector2 B = (creature.transform.position + bullet.transform.rotation * Vector3.right);
        Vector2 C = bullet.transform.position;
        int r = 1;

        if ((C.x - A.x) * (B.y - A.y) - (C.y - A.y) * (B.x - A.x) < 0) //Определение, с какой стороны летит пуля 
        {
            r = -1;
        }

        rigidBody.AddForce(bullet.transform.rotation * Quaternion.Euler(0, 0, 90 * r) * Vector3.right * bullet.speed * 70); //Уворот от пули
        if (bullet.owner)
        {
            target = bullet.owner;
        }
    }
    private void FixedUpdate()
    {
        MovementLogic();
    }
    public IEnumerator StepBool()
    {
        yield return Yielders.Get(10f);
        flyBool = true;
    }

    public void MovementLogic()
    {
        if (creature != null)
        {
            if (move)
            {
                rigidBody.AddForce(moveDir * Quaternion.Euler(0, 0, Random.Range(-90, 90)) * Vector3.right * Time.fixedDeltaTime * creature.moveSpeed);
                if (flyBool)
                {
                    AudioPlayer.Instance.PlaySoundFX(flyClip, audioSource);
                    flyBool = false;
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
        if (move)
        {
            Vector2 vect = movePosition - creature._transform.position;
            moveDir = Quaternion.FromToRotation(Vector3.right, vect);

            var angle = (Quaternion.Angle(creature.transform.rotation, moveDir));// owner._transform.eulerAngles.z - look - owner.swing)%360;
            if (Mathf.Abs(angle) >= 5)
            {
                creature.transform.rotation = Quaternion.RotateTowards(creature.transform.rotation, moveDir, turnSpeed * Time.deltaTime);
            }
            else
            {
                creature.transform.rotation = moveDir;
            }
            //creature._transform.eulerAngles = new Vector3(0, 0, 90 - direction);
            if (Vector2.Distance(_transform.position, movePosition) < 25)
            {
                animator.speed = 0;
                move = false;
            }
        }
        if (target != null)
        {
            Escape();
        }
        else
        {
            Idle();
        }
        _transform.position = creature._transform.position;
    }
    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Human"))
        {
            if ((target == null))
            {
                target = collision.GetComponent<Human>();
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
                movePosition = (Vector2)creature._transform.position + new Vector2(Random.Range(-creature.visionRange, creature.visionRange), Random.Range(-creature.visionRange, creature.visionRange));
            }
        }
    }
    public void FindTarget()
    {
        while ((targetContainer.Count > 0) && (target == null))
        {
            var obj = targetContainer.Dequeue();
            if ((obj != null) && (Vector2.Distance(creature._transform.position, obj._transform.position) < creature.visionRange))
            {
                target = obj;
            }
        }
    }
    public void Escape() //Бегство
    {
        float dist = Vector2.Distance(creature._transform.position, target._transform.position);
        if (dist > creature.visionRange * 2.5f)
        {
            target = null;
            animator.speed = 0;
            move = false;
        }
        else
        {
            moveDir = Quaternion.FromToRotation(Vector3.right, creature.transform.position - target._transform.position);
            move = true;
            animator.speed = 1;
            movePosition = creature.transform.position + (creature.transform.position - target._transform.position).normalized * 64;
        }
    }
}
