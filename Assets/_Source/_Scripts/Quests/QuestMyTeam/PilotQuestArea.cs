using UnityEngine;

public class PilotQuestArea : MonoBehaviour
{
    private Human player;
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (player && collision.gameObject == player.gameObject)
        {
            QuestMyTeam.instance.UpdateQuest();
            Destroy(gameObject);
        }
    }
    private void Start()
    {
        player = WorldController.Instance.playerControl.human;
    }
    private void Update()
    {
        if (QuestMyTeam.instance.pilot != null)
        {
            transform.position = QuestMyTeam.instance.pilot.transform.position;
        }
        else
        {
            Destroy(gameObject);
        }
    }
}
