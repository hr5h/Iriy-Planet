using ObjectPooling;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AddressableTools;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using System;
using JetBrains.Annotations;

/// <summary>
/// Хранилище пулов игровых объектов
/// </summary>
[System.Serializable]
public class PoolStorage
{
    [Serializable]
    public class PoolData<T> where T : MonoBehaviour
    {
        private T _prefab;
        [SerializeField]
        public ComponentReference<T> componentReference;
        public MonoBehaviourPool<T> pool;
        public int preloadCount;

        public void Init()
        {
            _prefab = AddressableLoader.LoadAsset(componentReference);
            pool = new MonoBehaviourPool<T>(_prefab, preloadCount);
        }
        public void Release()
        {
            _prefab = null;
            componentReference.ReleaseAsset();
        }
    }

    public PoolData<Bullet> bulletData;
    public PoolData<BloodPool> bloodPoolData;
    public PoolData<BloodBurst> bloodBurstData;
    public PoolData<BloodSplash> bloodSplashData;
    public PoolData<BarrierSplash> barrierSplashData;
    public PoolData<Stone> stoneData;
    public PoolData<Plant> plantData;
    public PoolData<BulletTrail> bulletTrailData;

    public void InitPools()
    {
        bulletData.Init();
        bloodPoolData.Init();
        bloodBurstData.Init();
        bloodSplashData.Init();
        barrierSplashData.Init();
        plantData.Init();
        stoneData.Init();
        bulletTrailData.Init();
    }
    public void ReleasePools()
    {
        bulletData.Release();
        bloodPoolData.Release();
        bloodBurstData.Release();
        bloodSplashData.Release();
        barrierSplashData.Release();
        plantData.Release();
        stoneData.Release();
        bulletTrailData.Release();
    }
}