using UnityEngine;

public class BarrierSplash : ParticleObject
{
    private void OnParticleSystemStopped()
    {
        WorldController.Instance.poolStorage.barrierSplashData.pool.Return(this);
    }
}