using UnityEngine;

public class ButtonF : MonoBehaviour
{
    private Canvas canvas;
    private void Awake()
    {
        canvas = GetComponent<Canvas>();
    }
    private void Start()
    {
        WorldController.Instance.playerControl.OnWindowChanged.AddListener(SetVisible); //Подписка на событие обновления окон
        SetVisible();
    }
    private void SetVisible()
    {
        if (Command.AllWindowsClosed())
        {
            canvas.enabled = true;
        }
        else
        {
            canvas.enabled = false;
        }
    }
}
