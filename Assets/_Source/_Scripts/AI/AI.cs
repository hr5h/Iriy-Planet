using System.Collections.Generic;
using UnityEngine;

public class AI : MonoBehaviour
{
    public Character target; //Цель
    public Queue<Character> targetContainer = new Queue<Character>(); //Набор целей нахождении нескольких противников в зоне видимости
    public bool move; //Нужно ли двигаться к movePosition
    public Character creature; //Обладатель ИИ

    [HideInInspector] public CircleCollider2D visionCollider; //Поле зрения, реализованное посредством Collider2D
    [HideInInspector] public Rigidbody2D visionRigidBody;
    [HideInInspector] public Rigidbody2D rigidBody; //Тело персонажа
    [HideInInspector] public Transform _transform;
    public float direction; //Направление движения
    public virtual void SetTarget(Character other)
    {
        target = other;
    }
    public void Awake()
    {
        visionCollider = GetComponent<CircleCollider2D>();
        visionRigidBody = GetComponent<Rigidbody2D>();
        _transform = transform;
    }
}
