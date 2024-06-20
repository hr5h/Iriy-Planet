using UnityEngine;

public class Mineral : Item
{
    public MineralData data;
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
            Logger.Debug("Нет данных о минерале!");
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
        if (!human.mainHero && data.dontPickup)
            return false;

        if (human && human.mainHero)
        {
            Command.ShowMessage("Получен предмет:", data.Title, Color.white, 1, false);
        }
        var state = new MineralState();
        state.data = data;
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
