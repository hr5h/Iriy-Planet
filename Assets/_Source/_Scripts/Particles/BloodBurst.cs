using System;
using UnityEngine;

public class BloodBurst : ParticleObject
{
    private void OnParticleSystemStopped()
    {
        WorldController.Instance.poolStorage.bloodBurstData.pool.Return(this);
    }
}