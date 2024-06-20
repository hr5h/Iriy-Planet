using UnityEngine;

public class ItemDefault : Item
{
    public ItemData data;
    [HideInInspector] public CircleCollider2D circleCollider;
    protected override void Awake()
    {
        circleCollider = GetComponent<CircleCollider2D>();
        base.Awake();
    }
    public override void Init()
    {
        if (!data)
        {
            Logger.Debug("Нет данных о предмете!");
            return;
        }
        var width = data.Icon.bounds.size.x;
        name = data.Title;
        circleCollider.radius = width / 2;
        spriteRenderer.sprite = data.Icon;
        //rigidBody.simulated = true;
    }
    public override bool PickUp(Human human)
    {
        if (human && human.mainHero)
        {
            Command.ShowMessage("Получен предмет:", data.Title, Color.white, 1, false);
        }
        if (!human.mainHero && data.questItem) //НПС не могут поднимать квестовые предметы
            return false;
        var state = new ItemState
        {
            data = data
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
