using UnityEngine;

public class Ammo : Item
{
    public AmmoData data;
    public int count;
    [HideInInspector] public BoxCollider2D boxCollider;
    protected override void Awake()
    {
        boxCollider = GetComponent<BoxCollider2D>();
        base.Awake();
    }
    public override void Init()
    {
        if (!data)
        {
            Logger.Debug("Нет данных о боеприпасах!");
            return;
        }
        var width = data.Icon.bounds.size.x;
        var height = data.Icon.bounds.size.y;
        boxCollider.size = new Vector2(width, height);
        name = data.Title;
        spriteRenderer.sprite = data.Icon;
        //rigidBody.simulated = true;
        base.Init();
    }
    public override bool PickUp(Human human)
    {
        if (!human.mainHero && data.dontPickup)
            return false;
        if (human && human.mainHero)
        {
            Command.ShowMessage("Получен предмет:", data.Title, Color.white, 1, false);
        }
        if (human.inventory.IsEmpty(data))
        {
            var state = new AmmoState
            {
                data = data,
                count = count
            };
            human.TakeAmmo(data.ammoType, count);

            if (human.inventory.Add(state))
            {
                Destroy(gameObject);
                return true;
            }
            count = state.count;
            human.TakeAmmo(data.ammoType, -count);
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
