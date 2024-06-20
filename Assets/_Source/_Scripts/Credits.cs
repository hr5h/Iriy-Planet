using UnityEngine;
using UnityEngine.SceneManagement;

public class Credits : MonoBehaviour
{
    public float speed;

    public AudioSource audioSource;

    //private void Start()
    //{
    //    //StartCoroutine(CoroutineEndText());
    //}

    private void Update()
    {
        //Logger.Debug(transform.localPosition);
        transform.localPosition = new Vector3(transform.localPosition.x, transform.localPosition.y + speed * Time.deltaTime, transform.localPosition.z);
        //if (transform.localPosition.y >= 1080)
        //{
        //    //audioSource.Stop();
        //    SceneManager.LoadScene(0);
        //}
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        SceneManager.LoadScene(0);
    }

    //private IEnumerator CoroutineEndText()
    //{
    //    yield return Yielders.Get(21);
    //    credits.transform.position = position.position;
    //    StartCoroutine(AudioLess());
    //    SceneManager.LoadScene(0);
    //}

    //IEnumerator AudioLess()
    //{
    //    yield return Yielders.Get(0.05f);
    //    if (audioSource.volume >= 0)
    //    {
    //        audioSource.volume -= 0.05f;
    //        StartCoroutine(AudioLess());
    //    }
    //    else
    //    {
    //        audioSource.Stop();
    //    }
    //}
}