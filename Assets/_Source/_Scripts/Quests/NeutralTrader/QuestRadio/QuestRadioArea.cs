using System.Collections.Generic;
using UnityEngine;

//«она, в пределах которой нужно уничтожить всех монстров, чтобы торговец был спасен
public class QuestRadioArea : MonoBehaviour
{

    public List<Monster> monsters = new List<Monster>();

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.TryGetComponent(out Monster monster))
        {
            monsters.Add(monster);
            monster.OnDeath.AddListener(CheckMonsters);
        }
    }
    private void CheckMonsters(Damageable monster = null, Damageable killer = null)
    {
        if (monster)
        {
            var m = monster as Monster;
            if (monsters.Contains(m))
            {
                monsters.Remove(m);
            }
        }
        if (monsters.Count == 0)
        {
            QuestRadio.instance.UpdateQuest();
            Destroy(gameObject);
        }
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (TryGetComponent(out Monster monster))
        {
            monster.OnDeath.RemoveListener(CheckMonsters);
            CheckMonsters(monster);
        }
    }
    private void Start()
    {
        transform.position = QuestRadio.instance.questTrader.transform.position;
        transform.SetParent(QuestRadio.instance.questTrader.transform); //ѕеремещатьс¤ за торговцем
    }
}
