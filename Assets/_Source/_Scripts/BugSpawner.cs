using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BugSpawner : MonoBehaviour, IUpdatable
{
    //Пока что только для теста
    public float spawnTimeOut;
    private float time;
    private Camera mainCamera;
    public GameObject bugPref;
    public GameObject aiPref;
    public Human player;
    public WorldController controller;
    public List<Monster> monsters;
    public int monsterLimit = 7;
    public static BugSpawner instance;
    private void Awake()
    {
        instance = this;
    }
    void Start()
    {
        mainCamera = Camera.main;
        time = spawnTimeOut;
        //Logger.Debug(time);
    }

    public void TeleportBug()
    {
        int line = Random.Range(0, 4);
        Vector2 pos = mainCamera.transform.position;
        switch (line)
        {
            case 0: pos += new Vector2(-1080, Random.Range(-540, 540)); break;
            case 1: pos += new Vector2(1080, Random.Range(-540, 540)); break;
            case 2: pos += new Vector2(Random.Range(-1080, 1080), -540); break;
            case 3: pos += new Vector2(Random.Range(-1080, 1080), 540); break;
        }
        if (!WorldController.Instance.IsQuestArea(pos))
        {
            Monster CloseMonster = null;
            float maxRange = float.MinValue;
            foreach (Monster m in monsters)
            {
                if (!m.notForSpawner)
                {
                    if (maxRange < m.Range(player))
                    {
                        CloseMonster = m;
                        maxRange = m.Range(player);
                    }
                }
            }
            if (maxRange > 540)
            {
                CloseMonster.GetComponent<Transform>().position = pos;
            }
        }
    }
    public void SpawnBug()
    {

        int line = Random.Range(0, 4);
        Vector2 pos = mainCamera.transform.position;
        switch (line)
        {
            case 0: pos += new Vector2(-1080, Random.Range(-540, 540)); break;
            case 1: pos += new Vector2(1080, Random.Range(-540, 540)); break;
            case 2: pos += new Vector2(Random.Range(-1080, 1080), -540); break;
            case 3: pos += new Vector2(Random.Range(-1080, 1080), 540); break;
        }
        if (!WorldController.Instance.IsQuestArea(pos))
        {
            var ai = Instantiate(aiPref, pos, transform.rotation).GetComponent<MonsterAI>();
            ai.target = player;
            ai.monster = Instantiate(bugPref, pos, transform.rotation).GetComponent<Monster>();
            if (Random.Range(0, 2) == 0)
            {
                ai.monster.dropItem = "bugHead";
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
        if (player != null)
        {
            if (time > 0)
            {
                time = Mathf.Max(0, time - Time.deltaTime);
                if (time == 0)
                {
                    time = spawnTimeOut;
                    if (monsters.Count < monsterLimit)
                        SpawnBug();
                    else
                        TeleportBug();
                }
            }
        }
        else
        {
            Destroy(this);
        }
    }
}
