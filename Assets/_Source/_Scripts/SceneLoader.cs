using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Асинхронная загрузку сцен
/// </summary>
public class SceneLoader : MonoBehaviour
{
    private static SceneLoader _instance;
    public static SceneLoader Instance { 
        get
        {
            if (_instance != null)
            {
                return _instance;
            }
            return new GameObject(nameof(SceneLoader)).AddComponent<SceneLoader>();
        }
    }
    private readonly Dictionary<string, AsyncOperation> _preloadedScenes = new Dictionary<string, AsyncOperation>();
    private readonly Dictionary<string, AsyncOperation> _loadingScenes = new Dictionary<string, AsyncOperation>();
    private void Awake()
    {
        if (_instance != null)
        {
            Destroy(gameObject);
            return;
        }
        SceneManager.activeSceneChanged += OnActiveSceneChanged;
        _instance = this;
        DontDestroyOnLoad(gameObject);
    }

    private void OnActiveSceneChanged(Scene arg0, Scene arg1)
    {
        if (_preloadedScenes.ContainsKey(arg1.name))
            _preloadedScenes.Remove(arg1.name);

        if (_loadingScenes.ContainsKey(arg1.name))
            _loadingScenes.Remove(arg1.name);
    }

    public void PreloadScene(string scene, bool allowActivation = false)
    {
        if (!_preloadedScenes.ContainsKey(scene))
        {
            StartCoroutine(PreloadSceneCoroutine(scene, allowActivation));
        }
    }
    public void LoadScene(string scene)
    {
        if (_loadingScenes.ContainsKey(scene))
        {
            _loadingScenes[scene].allowSceneActivation = true;
        }
        else
        {
            PreloadScene(scene, true);
        }
    }

    IEnumerator PreloadSceneCoroutine(string scene, bool allowActivation)
    {
        if (_loadingScenes.ContainsKey(scene)) yield break;

        bool preloaded = false;
        yield return null;

        AsyncOperation asyncOperation = SceneManager.LoadSceneAsync(scene);

        _loadingScenes.Add(scene, asyncOperation);

        asyncOperation.allowSceneActivation = allowActivation;

        while (!asyncOperation.isDone)
        {
            if (asyncOperation.progress >= 0.9f)
            {
                if (!preloaded)
                {
                    preloaded = true;
                    _preloadedScenes.Add(scene, asyncOperation);
                }
            }

            yield return null;
        }
    }
}
