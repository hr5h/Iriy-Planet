using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class CampArea : MonoBehaviour
{
    public UnityEvent<Human> OnHumanEnter; //НПС зашел в лагерь
    public UnityEvent OnDead; //Костер потух
    public List<Human> humans = new List<Human>();

    private void OnDestroy()
    {
        OnDead?.Invoke();
    }
    private void OnTriggerEnter2D(Collider2D collision) //Вход в лагерь
    {
        if (collision.gameObject.TryGetComponent(out Human obj))
        {
            OnHumanEnter?.Invoke(obj);
            humans.Add(obj);
            if (obj.mainHero)
            {
                WorldController.Instance.arrow.SetActive(false);
            }
            obj.inCamp = true;
            obj.regen += 1;
        }
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.TryGetComponent(out Human obj))
        {
            OnHumanEnter?.Invoke(obj);
            if (obj.mainHero)
            {
                WorldController.Instance.arrow.SetActive(false);
            }
            obj.inCamp = false;
            obj.regen -= 1;
        }
    }
}
