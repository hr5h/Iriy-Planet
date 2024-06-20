using ObjectPooling;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Object = UnityEngine.Object;

namespace ObjectPooling
{
    public class MonoBehaviourPool<T> where T : MonoBehaviour
    {
        private T _prefab;
        private readonly Queue<T> _pool = new Queue<T>();
        private readonly List<T> _active = new List<T>();
        private Transform _container;

        public MonoBehaviourPool(T prefab, int preloadCount)
        {
            _prefab = prefab;
            _container = new GameObject(prefab.name + "(Pool)").transform;
            for (int i = 0; i < preloadCount; ++i)
            {
                Return(Preload());
            }
        }

        public T Get()
        {
            T @object = _pool.Count > 0 ? _pool.Dequeue() : Preload();
            @object.gameObject.SetActive(true);
            _active.Add(@object);

            return @object;
        }
        public void Return(T @object)
        {
            @object.gameObject.SetActive(false);
            _pool.Enqueue(@object);
            _active.Remove(@object);
        }

        private T Preload() => Object.Instantiate(_prefab, _container);
    }
}