using Game.Sounds;
using System.Collections.Generic;
using UnityEngine;

public class MessageController : MonoBehaviour
{
    [HideInInspector] public HistoryController HC;
    public static MessageController instance;
    public Transform messagePosition;
    public GameObject messagePref;
    public GameObject overlay;
    public List<Message> messages = new List<Message>();

    //AudioSource
    public AudioSource audioSource;
    //AudioClip
    public AudioClip showMessageClip;

    private void Awake()
    {
        instance = this;
    }
    private void Start()
    {
        HC = HistoryController.instance;
    }
    public void OnMessageDestroy(Message msg)
    {
        for (int i = 0; i < messages.Count; i++)
        {
            if (messages[i] != msg)
            {
                messages[i].transform.localPosition = messages[i].transform.localPosition + (new Vector3(0, -msg.height - 2));
            }
            else
            {
                messages.Remove(msg);
                break;
            }
        }
        msg.TimeIsOver.RemoveListener(OnMessageDestroy);
    }
    public Message ShowMessage(string title, string text, Color col, float lifetime = 10, bool inHistory = true)
    {
        if (!Overlay.Exists) return null;
        if (inHistory)
        {
            AudioPlayer.Instance.PlayUI(showMessageClip);
            HistoryController.instance.Add(title + "\n" + text);
        }
        var msg = Instantiate(messagePref, messagePosition.transform).GetComponent<Message>();
        //msg.transform.localPosition = new Vector2(messagePosition.localPosition.x, messagePosition.localPosition.y);
        msg.Create(title, text, col, lifetime);
        foreach (var x in messages)
        {
            x.transform.localPosition = x.transform.localPosition + (new Vector3(0, msg.height + 2));
        }
        messages.Add(msg);
        msg.TimeIsOver.AddListener(OnMessageDestroy);
        return msg;
    }
}
