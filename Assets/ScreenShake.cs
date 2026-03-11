using System;
using UnityEngine;
using Random = UnityEngine.Random;

public class ScreenShake : MonoBehaviour
{
    [SerializeField, Min(0)] private float maxShakeTime;
    [SerializeField, Min(0)] private float decreaseFactor;
    [SerializeField, Min(1)] private float shakeMod = 200f;
    [SerializeField] private Transform head;
    private float shakeTime;
    private float shakeIntensity;

    private bool isDead;

    private Vector3 defaultPosition;
    private void Start()
    {
        defaultPosition = transform.localPosition;
    }

    private void Update()
    {
        if (shakeTime > 0) {
            transform.localPosition = defaultPosition + Random.insideUnitSphere * shakeIntensity;
            shakeTime -= Time.deltaTime * decreaseFactor;

        } else {
            shakeTime = 0.0f;
            transform.localPosition = defaultPosition;
        }

        if (isDead)
        {
            transform.position = head.position;
            transform.rotation = head.rotation;
        }
    }

    public void ShakeScreen(int intensity)
    {
        float amount = intensity/shakeMod;
        Debug.Log(amount);
        shakeTime = maxShakeTime;
        shakeIntensity = amount;
    }

    public void SetDead(bool value)
    {
        isDead = value;
    }
}
