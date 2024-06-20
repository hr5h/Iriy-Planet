using System;
using UnityEngine;

public class BloodSplash : ParticleObject
{
    private void OnParticleSystemStopped()
    {
        WorldController.Instance.poolStorage.bloodSplashData.pool.Return(this);
    }
}