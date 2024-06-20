using System.Collections;
using UnityEngine;

public class HumanLegs : MonoBehaviour, IUpdatable
{
    public float rate;
    public float time;

    public Human human;
    SpriteRenderer spriteRenderer;
    Transform _transform;
    public void Awake()
    {
        _transform = transform;
        spriteRenderer = GetComponent<SpriteRenderer>();
    }
    public void Rotate(float x, float y)
    {
        switch ((x, y))
        {
            case (1, 0): _transform.eulerAngles = new Vector3(0, 0, 0); break;
            case (1, 1): _transform.eulerAngles = new Vector3(0, 0, 45); break;
            case (0, 1): _transform.eulerAngles = new Vector3(0, 0, 90); break;
            case (-1, 1): _transform.eulerAngles = new Vector3(0, 0, 135); break;
            case (-1, 0): _transform.eulerAngles = new Vector3(0, 0, 180); break;
            case (-1, -1): _transform.eulerAngles = new Vector3(0, 0, 226); break;
            case (0, -1): _transform.eulerAngles = new Vector3(0, 0, 270); break;
            case (1, -1): _transform.eulerAngles = new Vector3(0, 0, 315); break;
        }
    }
    public void Step()
    {
        spriteRenderer.flipY = !spriteRenderer.flipY;
        time = rate;
    }
    public void Hide()
    {
        if (spriteRenderer.isVisible)
        {
            spriteRenderer.enabled = false;
        }
    }
    public void Show()
    {
        if (!spriteRenderer.isVisible)
        {
            Step();
            spriteRenderer.enabled = true;
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
        _transform.position = human._transform.position;
        if (time <= 0)
        {
            Step();
        }
    }
}
