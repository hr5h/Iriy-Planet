using UnityEngine;

public class MeasuringArea : MonoBehaviour
{
    public Human player;
    public bool inArea;
    public GameObject tip; //Подсказка
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject == player.gameObject && Command.HasItem(player.inventory, "professordevice"))
        {
            inArea = true;
            tip.SetActive(true);
        }
    }
    public void StartMeasuring()
    {
        QuestScienceExpiriens.instance.Measuring();
        Destroy(gameObject);
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject == player.gameObject)
        {
            inArea = false;
            tip.SetActive(false);
        }
    }
    private void Start()
    {
        player = WorldController.Instance.playerControl.human;
    }
    private void Update()
    {
        if (inArea && Input.GetKeyDown(KeyCode.Space))
        {
            StartMeasuring();
        }
    }
}
