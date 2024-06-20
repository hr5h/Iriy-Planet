using System.Text;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class HistoryController : MonoBehaviour
{
    public static HistoryController instance;
    public StringBuilder text = new StringBuilder("", 1000);
    public GameObject historyUI;
    public float maxSize;
    public bool open;
    [HideInInspector] public UnityEvent OnHistoryClose; ///Событие закрытия истории
    [HideInInspector] public UnityEvent OnHistoryOpen; ///Событие открытия истории
    [HideInInspector] public UnityEvent OnTextUpdate; ///Событие обновления текста

    private Canvas _historyCanvas;
    [SerializeField] private ScrollRect _historyScroll;
    private void Awake()
    {
        instance = this;
        open = false;
        _historyCanvas = historyUI.GetComponent<Canvas>();
    }
    public void Add(string str)
    {
        //Logger.Debug(str);
        while (text.Length + str.Length + 3 > maxSize)
        {
            text.Remove(0, text.ToString().IndexOf("\n \n") + 3);
        }
        text.Append(str + "\n \n");
        if (open)
        {
            OnTextUpdate?.Invoke();
        }
    }
    public void HistoryOpen()
    {
        Command.CloseAllWindow();

        _historyCanvas.enabled = true;
        _historyScroll.enabled = true;
        open = true;
        OnTextUpdate?.Invoke();
        OnHistoryOpen?.Invoke();
    }
    public void HistoryClose()
    {
        _historyCanvas.enabled = false;
        _historyScroll.enabled = false;
        open = false;
        OnHistoryClose?.Invoke();
    }
}
