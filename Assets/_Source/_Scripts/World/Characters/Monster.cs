using Game.Sounds;
using Mono.Cecil;
using UnityEngine;

public class Monster : Character
{
    public float damage; //Урон
    public float rate; //Интервал атаки
    public float rateCurrent; //Текущий откат атаки
    public float range; //Дальность атаки
    public bool reload; //Перезарядка атаки
    public float direction; //Направление движения и взгляда
    public float swing; //Покачивание при движении
    public float swingCoef;
    public float maxSwing; //Максимальный угол покачивания
    public bool isSpawned = false; //Был ли он заспавлен

    //AudioClip
    public AudioClip attackClip;
    public AudioClip moveClip;


    public void Attack(Character target)
    {
        AudioPlayer.Instance.PlaySoundFX(attackClip, basicSource);
        target.TakeDamage(damage, this);
        worldController.SpawnBloodSplash(transform.position + transform.right * range / 2, target.bloodColor);
        //Заставить НПС бороться с монстром
        if ((target is Human) && ((target as Human).ai))
        {
            var other = ((target as Human).ai as HumanAI);
            other.target = this;
        }
    }
    public override void ManualUpdate()
    {
        if (Mathf.Abs(swing) > maxSwing)
        {
            swing = swingCoef * maxSwing;
            swingCoef *= -1;
        }
        if (rateCurrent > 0) //Перезарядка после атаки
        {
            rateCurrent = Mathf.Max(0, rateCurrent - Time.deltaTime);
            if (rateCurrent == 0) reload = false;
        }
        if (Time.frameCount % 13 == 0)
            UpdateCurrentChunk();
        base.ManualUpdate();
    }
    private void UpdateCurrentChunk()
    {
        var coord = Chunks.CalculateCurrentChunk(transform.position);
        if (coord != currentChunk)
        {
            Chunks.ChangeChunk(ref currentChunk, ref coord, this);
        }
    }
    protected override void AddEntityToWorld()
    {
        Entities.Add(this);
        currentChunk = Chunks.CalculateCurrentChunk(transform.position);
        Chunks.Add(currentChunk, this);
    }

    protected override void RemoveEntityFromWorld()
    {
        Entities.Remove(this);
        Chunks.Remove(currentChunk, this);
    }
}
