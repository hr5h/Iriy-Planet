using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NestAI : AI, IUpdatable
{
    public Monster monster; //Само гнездо
    private float time;
    public float timeout; //Интервал между появлением муравьев
    public int maxSummonsCount; //Максимальное количество подконтрольных монстров возле гнезда
    public List<Monster> summons = new List<Monster>();
    public GameObject summonPref;
    public GameObject summonAIPref;
    public CircleCollider2D zone;
    private void Start()
    {
        if (creature)
        {
            monster = creature as Monster;
        }
        time = 0;
        monster.ai = this;
        monster.OnDamage.AddListener(Agression);
        zone = GetComponent<CircleCollider2D>();
        zone.radius = monster.visionRange;
    }
    //private void OnTriggerEnter2D(Collider2D collision)
    //{
    //    if (collision.gameObject.name==summonPref.name)
    //    {
    //        var obj = collision.GetComponent<Monster>();
    //        SummonAdd(obj);
    //    }
    //}
    private void OnDestroy()
    {
        foreach (var x in summons) //Разрешение всем подконтрольным существам использоваться скриптом Spawner
        {
            BugSpawner.instance.monsterLimit--;
            x.notForSpawner = false;
        }
    }
    private void SummonAdd(Character summon) //Добавление существа в список подкронтрольных
    {
        if (!summons.Contains((Monster)summon))
        {
            BugSpawner.instance.monsterLimit++;
            summons.Add((Monster)summon);
            summon.notForSpawner = true;
            summon.OnDeath.AddListener(SummonRemove);
        }
    }
    private void SummonRemove(Damageable summon, Damageable killer = null) //Удаление существа из списка подконтрольных
    {
        BugSpawner.instance.monsterLimit--;
        ((Character)summon).notForSpawner = false;
        summon.OnDeath.RemoveListener(SummonRemove);
        summons.Remove((Monster)summon);
    }
    private void Agression(Damageable summon, Damageable agressor) //Если гнездо было атаковано
    {
        target = (Character)agressor;
        for (int i = 0; i < summons.Count; i++)
        {
            (summons[i].ai as MonsterAI).endlessHunting = true;
            (summons[i].ai as MonsterAI).isAgressive = true;
            summons[i].ai.target = target;
        }
    }
    private void Spawn() //Создание юнита
    {
        var summon = Instantiate(summonPref, transform.position + new Vector3(Random.Range(-5, 5), Random.Range(-5, 5), 0), Quaternion.Euler(0, 0, Random.Range(0, 360))).GetComponent<Monster>();
        var summonAI = Instantiate(summonAIPref, transform.position, transform.rotation).GetComponent<MonsterAI>();
        summon.isSpawned = true;
        summonAI.monster = summon;
        summonAI.target = target;
        summonAI.isAgressive = false;
        if (target)
        {
            summonAI.isAgressive = true;
            summonAI.endlessHunting = true;
        }
        SummonAdd(summon);
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
        if (target)
        {
            if (time == 0)
            {
                if (summons.Count < maxSummonsCount)
                {
                    time = timeout + Random.Range(-0.3f, 0.3f);
                    Spawn();
                }
            }
            if (Vector2.Distance(target.transform.position, transform.position) > monster.visionRange)
            {
                target = null;
                foreach (var x in summons)
                {
                    (x.ai as MonsterAI).endlessHunting = false;
                    (x.ai as MonsterAI).isAgressive = false;
                }
            }
        }
        else
        {
            if (time == 0)
            {
                time = (timeout + Random.Range(-0.3f, 0.3f)) * 2;
                foreach (var x in summons)
                {
                    if (Random.Range(0, 3) == 0)
                        if (x.ai.target == null)
                        {
                            (x.ai as MonsterAI).movePosition = transform.position + Quaternion.Euler(0, 0, Random.Range(0, 360)) * new Vector3(64f, 0, 0);
                            (x.ai as MonsterAI).move = true;
                            (x.ai as MonsterAI).animator.speed = 1;
                        }
                }
                if (summons.Count < maxSummonsCount / 2)
                {
                    Spawn();
                }
            }
        }
        if (time != 0)
        {
            time = Mathf.Max(0, time - Time.deltaTime);
        }
    }
}
