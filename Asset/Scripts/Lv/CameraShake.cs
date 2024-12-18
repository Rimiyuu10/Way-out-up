using System.Collections;
using UnityEngine;

public class CameraShake : MonoBehaviour
{
    public static CameraShake MyInstance;
    private Vector3 startPosition;
    private Coroutine currentShake;

    private void Awake()
    {
        MyInstance = this;
        startPosition = transform.position;
    }

    public IEnumerator Shake(float shakeTimer, AnimationCurve curve)
    {
        float timeUsed = 0f;

        while (timeUsed < shakeTimer)
        {
            timeUsed += Time.deltaTime;
            float strength = curve.Evaluate(timeUsed / shakeTimer);
            transform.position = startPosition + Random.insideUnitSphere * strength;
            yield return null;
        }
        transform.position = startPosition;
    }

    public void StartShake(float shakeTimer, AnimationCurve curve)
    {
        if (currentShake != null)
        {
            StopCoroutine(currentShake);
            transform.position = startPosition;
        }
        currentShake = StartCoroutine(Shake(shakeTimer, curve));
    }

    public void StopShake()
    {
        if (currentShake != null)
        {
            StopCoroutine(currentShake);
            transform.position = startPosition;
            currentShake = null;
        }
    }
}
