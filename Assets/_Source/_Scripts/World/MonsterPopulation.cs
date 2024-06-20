using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterPopulation : MonoBehaviour, IUpdatable
{
    // TODO MonsterPopulation попул¤ци¤ монстров на определенной территории. ћонстры не выход¤т за еЄ пределы
    private float time;
    public float timeout; //»нтервал перемещением попул¤ции
    public Color populationColor; //÷вет монстров в попул¤ции
    public Color bloodColor; //÷вет крови у монстров
    public bool changeBloodColor; //»змен¤ть ли цвет крови у монстров
    public float radius; //ќхват территории попул¤цией
    public List<Monster> monsters = new List<Monster>();
    private void Start()
    {
        foreach (var x in monsters)
        {
            BugSpawner.instance.monsterLimit++;
            x.rend.color = populationColor;
            x.notForSpawner = true;
            x.OnDeath.AddListener(Remove);
            if (changeBloodColor)
            {
                x.bloodColor = bloodColor;
            }
        }
        time = 0;
    }
    private void OnDestroy()
    {
        foreach (var x in monsters) //–азрешение всем подконтрольным существам использоватьс¤ скриптом Spawner
        {
            x.notForSpawner = false;
        }
    }
    private void Remove(Damageable monster, Damageable killer = null) //”даление существа из списка подконтрольных
    {
        BugSpawner.instance.monsterLimit--;
        ((Character)monster).notForSpawner = false;
        monster.OnDeath.RemoveListener(Remove);
        monsters.Remove((Monster)monster);
        if (monsters.Count == 0)
        {
            Destroy(gameObject);
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
        if (time == 0)
        {
            time = timeout + Random.Range(-0.1f, 0.1f);
            foreach (var x in monsters)
            {
                if (Vector2.Distance(x._transform.position, transform.position) > radius)
                    if (x.ai.target == null)
                    {
                        (x.ai as MonsterAI).movePosition = transform.position + Quaternion.Euler(0, 0, Random.Range(0, 360)) * new Vector3(radius * Random.Range(0f, 1f), 0, 0);
                        (x.ai as MonsterAI).move = true;
                        (x.ai as MonsterAI).animator.speed = 1;
                    }
            }
        }
        if (time != 0)
        {
            time = Mathf.Max(0, time - Time.deltaTime);
        }
    }
}
