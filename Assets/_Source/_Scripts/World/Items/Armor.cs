using UnityEngine;

public class Armor : Item
{
    public ArmorData data;
    [HideInInspector] public CircleCollider2D circleCollider;
    public float condition; //Текущая исправность
    protected override void Awake()
    {
        circleCollider = GetComponent<CircleCollider2D>();
        base.Awake();
    }
    public override void Init()
    {
        if (!data)
        {
            Logger.Debug("Нет данных о броне!");
            return;
        }
        var width = data.Icon.bounds.size.x;
        name = data.Title;
        circleCollider.radius = width / 2;
        spriteRenderer.sprite = data.Icon;
        //rigidBody.simulated = true;
        if (condition > data.conditionMax) condition = data.conditionMax;
    }
    public override bool PickUp(Human human)
    {
        /*
            if (human && human.mainHero)
            {
                Command.ShowMessage("Получен предмет:", data.Title, Color.white, 1, false);
            }
            if (human.armorData == null)
            {
                human.EquipArmor(data, condition);
                Destroy(gameObject);
            }
            else
            {
                PickUp(human);
            }
         */
        Logger.Debug("Подбор брони");
        if (!human.mainHero && data.dontPickup)
        {
            Logger.Debug("НПС не может подобрать этот предмет");
            return false;
        }
        var state = new ArmorState
        {
            data = data,
            condition = condition
        };
        if (human.inventory.Add(state))
        {
            Destroy(gameObject);
            return true;
        }
        return false;
    }
    public override void DestroyTimerStart(float t = 60)
    {
        if (!data.questItem)
        {
            base.DestroyTimerStart(t);
        }
    }
}