using UnityEngine;

public class MadmanBaseArea : MonoBehaviour
{
    private Human player;
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject == player.gameObject && !QuestSabotage.instance.started)
        {
            QuestSabotage.instance.PlayerInBase();
        }
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (player && collision.gameObject == player.gameObject && QuestSabotage.instance.bombPlanted)
        {
            QuestSabotage.instance.BombExplosion();
            Destroy(gameObject);
        }
    }
    void Start()
    {
        player = WorldController.Instance.playerControl.human;
    }
}
