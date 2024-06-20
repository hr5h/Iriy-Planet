using System.Collections;
using UnityEngine;

public class BombPlantArea : MonoBehaviour, IUpdatable
{
    public Human player;
    public bool inArea;
    public GameObject tip; //Подсказка
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject == player.gameObject)
        {
            inArea = true;
            if (QuestSabotage.instance.leaderIsGone && Command.HasItem(player.inventory, "bomb"))
            {
                ShowTip();
            }
        }
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject == player.gameObject)
        {
            inArea = false;
            ShowTip();

        }
    }
    public void ShowTip()
    {
        if (inArea)
            tip.SetActive(true);
        else
            tip.SetActive(false);
    }
    public void PlantBomb()
    {
        QuestSabotage.instance.BombPlant();
        Destroy(gameObject);
    }
    private void Start()
    {
        player = WorldController.Instance.playerControl.human;
        QuestSabotage.instance.OnLeaderMove.AddListener(ShowTip);
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
        if (QuestSabotage.instance.leaderIsGone && inArea && Input.GetKeyDown(KeyCode.Space))
        {
            PlantBomb();
        }
    }
}
