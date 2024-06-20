using Game.Sounds;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseGame : MonoBehaviour
{
    public static PauseGame Instance { get; private set; }

    public Settings settingsPanel;
    public GameObject cursor;
    public Canvas overlayCanvas;
    public PlayerControl playerControl;

    public AudioClip buttonClip;

    private Canvas _canvas;
    private Canvas _settingsCanvas;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }
    private void Start()
    {
        _canvas = GetComponent<Canvas>();
        _settingsCanvas = settingsPanel.GetComponent<Canvas>();
        _canvas.enabled = false;
        _settingsCanvas.enabled = false;
    }
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            ChangeState();
        }
    }

    public void ChangeState()
    {
        if (_settingsCanvas.enabled)
        {
            settingsPanel.Close();
        }
        else
        {
            if (_canvas.enabled)
                Resume();
            else
                Pause();
        }
    }

    public void Resume()
    {
        Application.targetFrameRate = -1;
        AudioPlayer.Instance.UnPauseAllSources();
        AudioPlayer.Instance.PlayUI(buttonClip);
        _canvas.enabled = false;

        Time.timeScale = 1f;
        if (playerControl)
        {
            overlayCanvas.enabled = true;
            playerControl.enabled = true;
        }
    }

    public void Restart()
    {
        AudioPlayer.Instance.PlayUI(buttonClip);
        SceneManager.LoadScene("SampleScene");
        Resume();
    }

    public void Pause()
    {
        Application.targetFrameRate = 30;
        AudioPlayer.Instance.PauseAllSources();
        AudioPlayer.Instance.PlayUI(buttonClip);
        _canvas.enabled = true;
        Time.timeScale = 0f;
        if (playerControl)
        {
            overlayCanvas.enabled = false;
            playerControl.enabled = false;
        }
    }

    public void Menu()
    {
        AudioPlayer.Instance.PlayUI(buttonClip);
        Resume();
        SceneManager.LoadScene(0);
    }

    public void Quit()
    {
        AudioPlayer.Instance.PlayUI(buttonClip);
        Application.Quit();
    }
}
