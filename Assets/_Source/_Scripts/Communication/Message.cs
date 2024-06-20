using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

public class Message : MonoBehaviour, IUpdatable
{
    public UnityEvent<Message> TimeIsOver;
    public float height;
    public float lifetime;
    public TextMeshProUGUI title;
    public TextMeshProUGUI content;
    public RectTransform background;

    public void Create(string t, string c, Color col, float time)
    {
        lifetime = time;

        title.text = t;
        content.text = c;

        title.color = col;
        content.color = col;

        title.ForceMeshUpdate();
        content.ForceMeshUpdate();

        height = title.GetRenderedValues()[1] + content.GetRenderedValues()[1];

        background.sizeDelta = new Vector2(Mathf.Max(title.GetRenderedValues()[0], content.GetRenderedValues()[0]), height);
        title.transform.localPosition = content.transform.localPosition + new Vector3(0, content.GetRenderedValues()[1]);
    }

    [ContextMenu("Получение размера")]
    private void Size()
    {
        title.ForceMeshUpdate();
        content.ForceMeshUpdate();

        background.sizeDelta = new Vector2(Mathf.Max(title.GetRenderedValues()[0], content.GetRenderedValues()[0]), title.GetRenderedValues()[1] + content.GetRenderedValues()[1]);
        title.transform.localPosition = content.transform.localPosition + new Vector3(0, content.GetRenderedValues()[1]);
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
        lifetime -= Time.deltaTime;
        if (lifetime <= 0)
        {
            TimeIsOver?.Invoke(this);
            Destroy(gameObject);
        }
    }
}
