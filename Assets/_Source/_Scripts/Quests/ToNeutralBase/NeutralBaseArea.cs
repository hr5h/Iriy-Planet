using UnityEngine;

public class NeutralBaseArea : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject == QuestToNeutralBase.instance.player)
        {
            Destroy(gameObject);
            QuestToNeutralBase.instance.StartCoroutine(QuestToNeutralBase.StartNeutralReplic());
        }
    }
}
