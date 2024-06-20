using UnityEngine;

public class WaspAI : MonsterAI
{
    public override void Hunt() //ќхота
    {
        if (!monster.reload)
        {
            hunting = true;
            float dist = Vector2.Distance(monster._transform.position, target._transform.position);
            if (dist < monster.range)
            {
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
