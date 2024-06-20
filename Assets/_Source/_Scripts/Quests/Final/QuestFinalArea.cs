using UnityEngine;

public class QuestFinalArea : MonoBehaviour
{
    private Human player;
    private Human pilot;
    public bool inArea;
    public GameObject tip;
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (player && collision.gameObject == player.gameObject)
        {
            inArea = true;
            tip.SetActive(true);
            return;
        }
        if (pilot && collision.gameObject == pilot.gameObject)
        {
            QuestFinal.instance.pilotInArea = true;
            return;
        }
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (player && collision.gameObject == player.gameObject)
        {
            inArea = false;
            tip.SetActive(false);
            return;
        }
        if (pilot && collision.gameObject == pilot.gameObject)
        {
            QuestFinal.instance.pilotInArea = false;
            return;
        }
    }
    void Start()
    {
        player = QuestFinal.instance.player;
        pilot = QuestFinal.instance.pilot;
    }
    private void Update()
    {
        if (inArea && Input.GetKeyDown(KeyCode.Space))
        {
            QuestFinal.instance.ShowFinalWindow();
        }
    }
}
