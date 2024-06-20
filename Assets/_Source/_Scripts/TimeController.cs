using System.Collections;
using UnityEngine;
using UnityEngine.Experimental.Rendering.Universal;

public class TimeController : MonoBehaviour, IUpdatable
{
    public WorldController controller;
    public Light2D globalLight;
    public int coef;
    public float speed;
    public float minIntesity;
    public float maxIntesity;
    public float time;
    private void Start()
    {
        time = maxIntesity;
        globalLight.intensity = maxIntesity;
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
        if (time > maxIntesity)
        {
            coef *= -1;
        }
        else
        {
            if (globalLight.intensity < minIntesity)
            {
                coef *= -1;
            }
        }
        time += speed * coef * Time.deltaTime;
        if (!controller.blackout)
            globalLight.intensity = time;
    }
}
