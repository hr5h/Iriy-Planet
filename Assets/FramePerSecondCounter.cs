using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class FramePerSecondCounter : MonoBehaviour, IUpdatable
{
    [SerializeField] TextMeshProUGUI textMeshProUGUI;
    public static FramePerSecondCounter Instance { get; private set; }
    private Canvas _canvas;

    public float updateInterval = 0.5F;
    private double lastInterval;
    private int frames;
    private float fps;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            _canvas = GetComponent<Canvas>();
            lastInterval = Time.realtimeSinceStartup;
            frames = 0;
        }
        else
        {
            Destroy(gameObject);
        }
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
        ++frames;
        float timeNow = Time.realtimeSinceStartup;
        if (timeNow > lastInterval + updateInterval)
        {
            fps = (float)(frames / (timeNow - lastInterval));
            frames = 0;
            lastInterval = timeNow;

            textMeshProUGUI.text = Mathf.RoundToInt(fps).ToString();
        }
    }
    public void Disable()
    {
        _canvas.enabled = false;
    }
    public void Enable()
    {
        _canvas.enabled = true;
    }
}
