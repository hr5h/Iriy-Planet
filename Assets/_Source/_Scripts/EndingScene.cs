using Game.Sounds;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class EndingScene : MonoBehaviour
{
    public TextMeshProUGUI tMesh;
    public int index;
    public AudioClip finalClip;

    IEnumerator Hide() //Исчезание текст
    {

        yield return Yielders.Get(0.05f);
        if (tMesh.alpha >= 0f)
        {
            tMesh.alpha -= 0.05f;
            StartCoroutine(Hide());
        }
        else
        {
            yield return Yielders.Get(1);
            Next();
        }
    }

    IEnumerator Show() //Появление текста
    {

        yield return Yielders.Get(0.05f);
        if (tMesh.alpha <= 1f)
        {
            tMesh.alpha += 0.05f;
            StartCoroutine(Show());
        }
        else
        {
            yield return Yielders.Get(2 + tMesh.text.Length / 30);
            StartCoroutine(Hide());
        }
    }

    IEnumerator BackToMenu() //Возврат в главное меню
    {
        yield return Yielders.Get(2);
        SceneLoader.Instance.LoadScene("Menu");
    }

    void Next() //Следующая фраза
    {
        index++;
        if (index < QuestFinal.ending.content.Count)
        {
            tMesh.text = QuestFinal.ending.content[index];
            StartCoroutine(Show());
        }
        else
        {
            StartCoroutine(BackToMenu());
        }
    }

    private void Start()
    {
        Time.timeScale = 1;
        tMesh = GetComponent<TextMeshProUGUI>();
        tMesh.text = QuestFinal.ending.content[0];
        tMesh.alpha = 0;
        AudioPlayer.Instance.PlayBackgroundMusic(finalClip);
        StartCoroutine(Show());
    }
}
