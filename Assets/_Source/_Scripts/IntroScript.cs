using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class IntroScript : MonoBehaviour
{
    public GameObject skipText;
    private TextMeshProUGUI _textMesh;
    private void Start()
    {
        _textMesh = skipText.transform.GetChild(0).GetComponent<TextMeshProUGUI>();
        StartCoroutine(CoroutineText());
    }

    void Update()
    {
        if (Input.anyKeyDown)
            if (skipText.activeInHierarchy)
            {
                SceneLoader.Instance.LoadScene("SampleScene");
                _textMesh.text = "Загрузка...";
            }
            else
                skipText.SetActive(true);
    }

    private IEnumerator CoroutineText()
    {
        yield return Yielders.Get(66);
        skipText.SetActive(true);
    }
}
