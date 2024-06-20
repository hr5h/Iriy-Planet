using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

[RequireComponent(typeof(Camera))]
public class ManualCameraRenderer : MonoBehaviour, IUpdatable
{
    public float fps = 20;
    float elapsed;
    Camera cam;

    void Start()
    {
        cam = GetComponent<Camera>();
        //if (cam.TryGetCullingParameters(out var cullingParameters))
        //{
        //    cullingParameters.cullingOptions = CullingOptions.ForceEvenIfCameraIsNotActive;
        //}
        cam.enabled = false;
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
        elapsed += Time.deltaTime;
        if (elapsed > 1 / fps)
        {
            elapsed = 0;
            cam.Render();
        }
    }
}
