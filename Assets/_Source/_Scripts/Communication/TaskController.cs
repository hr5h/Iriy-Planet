using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TaskController : MonoBehaviour
{
    public static TaskController instance;
    public GameObject taskPrefab;
    public List<Task> tasks = new List<Task>();
    public GameObject journal;
    public GameObject taskContainer;
    public RectTransform rect;
    public bool open;

    private Canvas _journalCanvas;
    [SerializeField] private ScrollRect _scrollRect;
    private void Awake()
    {
        instance = this;
        open = false;
        _journalCanvas = journal.GetComponent<Canvas>();
    }
    public void AddTask(string title, string description)
    {
        _journalCanvas.enabled = true;
        var task = Instantiate(taskPrefab, taskContainer.transform).GetComponent<Task>();
        task.Create(title, description);
        for (int i = 0; i < tasks.Count; i++)
        {
            task.transform.localPosition -= new Vector3(0, tasks[i].height);
        }
        tasks.Add(task);
        UpdateRect();
        if (!open)
        {
            _journalCanvas.enabled = false;
        }
    }
    public void RemoveTask(string title)
    {
        var ind = tasks.FindIndex(x => x.title == title);
        var h = tasks[ind].height;
        Destroy(tasks[ind].gameObject);
        for (int i = ind + 1; i < tasks.Count; i++)
        {
            tasks[i].transform.localPosition += new Vector3(0, h);
        }
        tasks.RemoveAt(ind);
        UpdateRect();
    }
    public void UpdateTask(string title, string description)
    {
        if (!Overlay.Exists) return;
        _journalCanvas.enabled = true;
        var ind = tasks.FindIndex(x => x.title == title);
        var t = tasks[ind];
        var h = t.height;
        t.Create(title, description);
        h -= t.height;
        for (int i = ind + 1; i < tasks.Count; i++)
        {
            tasks[i].transform.localPosition += new Vector3(0, h);
        }
        UpdateRect();
        if (!open)
        {
            _journalCanvas.enabled = false;
        }
    }
    public void UpdateRect()
    {
        float h = 0;
        foreach (var x in tasks)
        {
            h += x.height;
        }
        rect.sizeDelta = new Vector2(rect.sizeDelta.x, h);
    }
    public void JournalOpen()
    {
        Command.CloseAllWindow();

        _journalCanvas.enabled = true;
        _scrollRect.enabled = true;
        open = true;
    }
    public void JournalClose()
    {
        _journalCanvas.enabled = false;
        _scrollRect.enabled = false;
        open = false;
    }
}
