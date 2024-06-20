using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cheats : MonoBehaviour, IUpdatable //Объект, выполняющий чит-команды
{
    private EntityRepository Entities => controller.Entities;
    public Human player; //Игрок
    public BugSpawner spawner; //Спавн монстров

    public WorldController controller;
    public CheatMSG message;

    public bool godmode = false; //Включен ли режим Бога
    public bool noreload; //Влючен ли режим без перезарядки
    public bool oneShot; //Убийство с одного выстрела
    public bool spawn; //Активен ли спавнер

    void SetGodmode() //Активирует или деактивирует неуязвимость
    {
        if (godmode)
        {
            player.defence -= 100;
            message.Show("Режим бога отключён");
        }
        else
        {
            player.RestoreHealth();
            player.defence += 100;
            message.Show("Режим бога включён");
        }
        godmode = !godmode;
    }

    void NoReload() //Отключает перезарядку оружия
    {
        if (noreload)
        {
            noreload = false;
            message.Show("Бесконечные патроны отключены");
        }
        else
        {
            noreload = true;
            message.Show("Бесконечные патроны включены");
        }
    }
    void SetSpawner() //Активирует и деактивирует спавн монстров
    {
        if (spawn)
        {
            spawner.spawnTimeOut *= -1;
            message.Show("Спаун монстров отключён");
        }
        else
        {
            spawner.spawnTimeOut *= -1;
            message.Show("Спаун монстров включён");
        }
        spawn = !spawn;

    }
    //TODO: реализовать фунцию TeleportToCamp()
    void TeleportToCamp() //Телепортирует игрока к случайному костру
    {
        //player.transform.position.Set = 
    }
    //TODO: реализовать фунцию TeleportToHuman()
    void TeleportToHuman() //Телепортирует игрока к случайному НПС
    {

    }
    //TODO: реализовать фунцию TeleportToStart()
    void TeleportToStart() //Возвращает игрока на стартовую позицию
    {
        //if(player != null)
        player.transform.position = new Vector3(0, 0, 0);
        message.Show("Телепортация на стартовую позицию");
    }
    void KillAllPeople() // Убивает всех НПС на карте
    {
        var humans = Entities.Humans;
        for (int i = humans.Count - 1; i >= 0; i--)
        {
            if (humans[i] != player)
            {
                humans[i].Death();
            }
        }
        message.Show("Убиты все НПС");
    }

    void KillAllMonster() // Убивает всех монстров на карте
    {
        var monsters = Entities.Monsters;
        for (int i = monsters.Count - 1; i >= 0; i--)
        {
            monsters[i].Death();
        }
        message.Show("Убиты все Монстры");
    }
    void KillPlayer() //Убивает игрока
    {
        if (player != null)
        {
            player.Death();
        }
        message.Show("Игрок убит");
    }
    void OneShot() //Активирует и деактивирует убийство с одного выстрела
    {
        if (oneShot)
        {
            player.damageCoef = 1;
            message.Show("Убийство с одного выстрела отключено");
        }
        else
        {
            player.damageCoef = 100;
            message.Show("Убийство с одного выстрела включено");
        }
        oneShot = !oneShot;
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
        if ((player != null) && (player.weapon != null))
        {
            if (noreload)
            {
                player.weapon.ammo = 30;
            }
        }

        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            SetGodmode();
        }
        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            NoReload();
        }
        if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            OneShot();
        }
        if (Input.GetKeyDown(KeyCode.Alpha4))
        {
            SetSpawner();
        }
        if (Input.GetKeyDown(KeyCode.Alpha5))
        {
            TeleportToCamp();
        }
        if (Input.GetKeyDown(KeyCode.Alpha6))
        {
            KillPlayer();
        }
        if (Input.GetKeyDown(KeyCode.Alpha7))
        {
            TeleportToStart();
        }
        if (Input.GetKeyDown(KeyCode.Alpha8))
        {
            KillAllPeople();
        }
        if (Input.GetKeyDown(KeyCode.Alpha9))
        {
            KillAllMonster();
        }
        if (Input.GetKeyDown(KeyCode.Alpha0))
        {
            TeleportToHuman();
        }
    }
}
