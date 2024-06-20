using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class Flashlight : MonoBehaviour, IUpdatable
{
    public Color col;
    public float time;
    public float timeCurrent;
    private Image rend;
    void Start()
    {
        rend = GetComponent<Image>();
        rend.color = new Color(col.r, col.g, col.b, 1);
    }

    public void Flash(float t, Color c) //Создать вспышку на экране
    {
        rend.color = c;
        col = c;
        time = t;
        timeCurrent = time;
    }

    private void OnEnable() => StartCoroutine(OnEnableCoroutine());
    private IEnumerator OnEnableCoroutine()
    {
        yield return null;
        WorldController.Instance.Updater.Add(this);
    }
    private void OnDisable()
    {
        WorldController.Instance.Updater.Remove(this);
    }
    public void ManualUpdate()
    {
        if (timeCurrent != 0)
        {
            timeCurrent = Mathf.Max(0, timeCurrent - Time.deltaTime);
            col = new Color(col.r, col.g, col.b, timeCurrent / time);
            rend.color = col;
        }
    }
}
