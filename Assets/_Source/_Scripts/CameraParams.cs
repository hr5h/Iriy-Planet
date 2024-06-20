using Cinemachine;
using UnityEngine;

public class CameraParams : MonoBehaviour
{
    private CinemachineVirtualCamera _virtualCamera;
    private CinemachineBasicMultiChannelPerlin _noise;
    private void Start()
    {
        _virtualCamera = gameObject.GetComponent<CinemachineVirtualCamera>();
        _noise = _virtualCamera.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();
    }

    public void EnableCameraShake()
    {
        _noise.m_AmplitudeGain = 50.0f;
    }

    public void DisableCameraShake()
    {
        _noise.m_AmplitudeGain = 0.0f;
    }
}
