using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Ходит за игроком, чтобы правильно воспроизводились 3D звуки (без учета поворота игрока)
/// </summary>
public class PlayerAudioListener : MonoBehaviour, IUpdatable
{
    public Human player;

    private void Awake()
    {
        transform.position = player.transform.position;
        player.OnDeath.AddListener(OnPlayerDeath);
    }

    private void OnPlayerDeath(Damageable owner, Damageable killer)
    {
        Destroy(gameObject);
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
        transform.position = player.transform.position;
    }
    private void OnDestroy()
    {
        var camera = Camera.main;
        if (camera != null)
        {
            var listener = camera.gameObject.GetComponent<AudioListener>();
            listener.enabled = true;
            if (EventManager.Instance != null)
            {
                EventManager.Instance.OnAudioListenerChanged?.Invoke(listener);
            }
        }
    }
}
