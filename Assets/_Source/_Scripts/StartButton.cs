using Game.Sounds;
using UnityEngine;
using UnityEngine.SceneManagement;

public class StartButton : MonoBehaviour
{
    public AudioSource audioSource;
    public AudioClip buttonClip;

    public void ButtonStart()
    {
        AudioPlayer.Instance.PlayUI(buttonClip);
        //SceneManager.LoadScene(1);
        SceneLoader.Instance.LoadScene("Intro");
    }

    public void ButtonCredits()
    {
        AudioPlayer.Instance.PlayUI(buttonClip);
        SceneManager.LoadScene(3);
    }

    public void ButtonExit()
    {
        AudioPlayer.Instance.PlayUI(buttonClip);
        Application.Quit();
    }

}