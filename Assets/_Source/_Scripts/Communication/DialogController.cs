using UnityEngine;
using UnityEngine.Events;

public class DialogController : MonoBehaviour
{
    public static DialogController instance;
    public Character speaker; //Собеседник
    public Dialog dialog;
    public GameObject dialogUI;
    public GameObject tradingButton;
    public bool open;
    public bool condition; //Условие для ответа
    public PlayerControl playerController;

    public UnityEvent Closed;
    private void Awake()
    {
        instance = this;
    }

    public void Open(Dialog dia)
    {
        dialog = dia;
        speaker = dia.speaker;
        open = true;
        Show();
        DialogUI.instance.speakerName.text = speaker.MyName;
    }
    public void Close()
    {
        playerController.showDialog = false;
        dialog = null;
        speaker = null;
        open = false;
        Closed?.Invoke();
        Hide();
    }
    public void Hide()
    {
        DialogUI.instance.Close();
    }
    public void Show()
    {
        if (speaker.canTrade)
        {
            tradingButton.SetActive(true);
        }
        else
        {
            tradingButton.SetActive(false);
        }
        DialogUI.instance.Open();
    }
}
