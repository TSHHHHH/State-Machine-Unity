using Cinemachine;
using System.Collections;
using UnityEngine;

public class MainVCam : MonoBehaviour
{
    private CinemachineBasicMultiChannelPerlin mMultiChannelPerlin;

    private void Awake()
    {
        ServiceLocater.RegisterService(this);

        mMultiChannelPerlin = GetComponent<CinemachineVirtualCamera>().GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();
    }

    private void OnDisable()
    {
        ServiceLocater.UnregisterService<MainVCam>();
    }

    public void TriggerCameraShake(float shakeStrength, float shakeDuration)
    {
        StartCoroutine(CameraShake(shakeStrength, shakeDuration));
    }

    private IEnumerator CameraShake(float shakeStrength, float shakeDuration)
    {
        mMultiChannelPerlin.m_AmplitudeGain = shakeStrength;

        yield return new WaitForSeconds(shakeDuration);

        mMultiChannelPerlin.m_AmplitudeGain = 0f;
    }
}
