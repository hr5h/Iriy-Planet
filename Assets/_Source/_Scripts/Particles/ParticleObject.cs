using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticleObject : MonoBehaviour
{
    private ParticleSystem _PS;

    public void Init(Vector2 position, Color color)
    {
        transform.position = position;
        var col = _PS.colorOverLifetime;
        col.color = new ParticleSystem.MinMaxGradient(color, color);
        _PS.Play();
    }

    private void Awake()
    {
        _PS = GetComponent<ParticleSystem>();
    }
}
