using System.Collections.Generic;
using UnityEngine;

public class Body : MonoBehaviour
{
    public Sprite defaut_stay;
    public Sprite default_stay_weapon;
    public Sprite default_reload;
    public List<Sprite> default_shoot = new List<Sprite>();

    public Sprite stay;
    public Sprite stay_weapon;
    public List<Sprite> shoot = new List<Sprite>();
    public Sprite reload;

    public Transform weapon_stay;
    public Transform weapon_shoot;
    public Transform weapon_reload;

    public List<Sprite> DefaultSkin = new List<Sprite>();
    public List<Sprite> MarauderSkin = new List<Sprite>();
    public List<Sprite> MechanicSkin = new List<Sprite>();
    public List<Sprite> ScientistSkin = new List<Sprite>();
    public List<Sprite> MadmanSkin = new List<Sprite>();
    public List<Sprite> MechQuest = new List<Sprite>();
    public List<Sprite> MechLeader = new List<Sprite>();

    public Human human;

    [HideInInspector] public SpriteRenderer spriteRenderer;

    public string mode;
    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }
    public void UpdateRender() //Обновить спрайт
    {
        int oneHanded = 0;
        if ((human.weapon != null) && (human.weapon.data.oneHanded)) oneHanded = 1;
        switch (mode)
        {
            case "stay": spriteRenderer.sprite = stay; break;
            case "stay_weapon": spriteRenderer.sprite = stay_weapon; human.weapon._transform.position = weapon_stay.position; human.weapon._transform.rotation = weapon_stay.rotation; break;
            case "shoot": spriteRenderer.sprite = shoot[oneHanded]; human.weapon._transform.position = weapon_shoot.position; human.weapon._transform.rotation = weapon_shoot.rotation; break;
            case "reload": spriteRenderer.sprite = reload; human.weapon._transform.position = weapon_reload.position; human.weapon._transform.rotation = weapon_reload.rotation; break;
        }
    }

    public void ChangeSkin(List<Sprite> skin) //Вернуть стандартный скин
    {
        stay = skin[0];
        stay_weapon = skin[1];
        shoot[0] = skin[2];
        shoot[1] = skin[3];
        reload = skin[0];

        UpdateRender();
    }
    public void Stay() //Стоять
    {
        mode = "stay";
        UpdateRender();
    }
    public void StayWeapon() //Стоять с оружием
    {
        mode = "stay_weapon";
        UpdateRender();
    }
    public void Shoot() //Стрелять
    {
        mode = "shoot";
        UpdateRender();
    }
    public void Reload() //Перезаряжать
    {
        mode = "reload";
        UpdateRender();
    }

}
