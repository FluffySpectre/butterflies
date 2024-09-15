using UnityEngine;

public class CameraShake : MonoBehaviour
{
    public float shakeDuration = 0f;
    public float shakeAmount = 0.7f;
    public float decreaseFactor = 1.0f;

    void Update()
    {
        if (shakeDuration > 0)
        {
            transform.localPosition += Random.insideUnitSphere * shakeAmount;

            shakeDuration -= Time.deltaTime * decreaseFactor;
        }
        else
        {
            shakeDuration = 0f;
        }
    }

    public void TriggerShake(float duration)
    {
        shakeDuration = duration;
    }
}
