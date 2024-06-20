using System.Collections;
using UnityEngine;

public class AudioSourceDelete : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(DestroyTime());
    }

    public IEnumerator DestroyTime()
    {
        yield return Yielders.Get(5);
        Destroy(gameObject);
    }

}
