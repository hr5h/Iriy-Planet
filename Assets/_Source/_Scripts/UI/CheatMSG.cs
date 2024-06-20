using System.Collections;
using TMPro;
using UnityEngine;

public class CheatMSG : MonoBehaviour, IUpdatable
{
    public float alpha;
    TextMeshProUGUI textMesh;
    void Start()
    {
        textMesh = GetComponent<TextMeshProUGUI>();
        textMesh.alpha = 0;
        alpha = 0;
    }

    public void Show(string text)
    {
        alpha = 1.5f;
        textMesh.text = text;
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
        if (alpha > 0)
        {
            alpha = Mathf.Max(0, alpha - Time.deltaTime);
            textMesh.alpha = alpha;
        }
    }
}
