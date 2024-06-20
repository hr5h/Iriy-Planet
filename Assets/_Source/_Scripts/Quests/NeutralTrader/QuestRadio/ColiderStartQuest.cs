using UnityEngine;

public class ColiderStartQuest : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Human"))
        {
            print("Start");
        }
    }
}
