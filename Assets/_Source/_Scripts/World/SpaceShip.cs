using System.Collections;
using UnityEngine;

public class SpaceShip : MonoBehaviour, IUpdatable
{
    private GameObject player;
    private SpriteRenderer rend;
    private CircleCollider2D circleCollider;
    void Start()
    {
        rend = GetComponent<SpriteRenderer>();
        circleCollider = GetComponent<CircleCollider2D>();
    }
    private void OnTriggerEnter2D(Collider2D collision) //Вход в триггер
    {
        if (collision.gameObject.name == "Player")
        {
            player = collision.gameObject;
        }
    }
    private void OnTriggerExit2D(Collider2D collision) //Выход из триггера
    {
        if (collision.gameObject.name == "Player")
        {
            player = null;
            rend.color = new Color(rend.color.r, rend.color.g, rend.color.b, 1);
        }
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
        if (player)
        {
            rend.color = new Color(rend.color.r, rend.color.g, rend.color.b, Mathf.Max(0.5f, Vector2.Distance(transform.position, player.transform.position) / circleCollider.radius / 4f));
        }
    }
}
