using Game.Sounds;
using UnityEngine;

public class GameStart : MonoBehaviour
{
    public AudioClip menuMusic;
    void Start()
    {
        Application.targetFrameRate = 30;
        SceneLoader.Instance.PreloadScene("Intro");
        SceneLoader.Instance.PreloadScene("SampleScene");
        AudioPlayer.Instance.PlayBackgroundMusic(menuMusic);
    }
}
