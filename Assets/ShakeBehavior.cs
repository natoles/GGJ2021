using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShakeBehavior : MonoBehaviour
{
    // Transform of the GameObject you want to shake
    private Transform objectTransform;

    // Desired duration of the shake effect
    private float shakeDuration = 0f;

    // A measure of magnitude for the shake. Tweak based on your preference
    private float shakeMagnitude = 10f;

    // A measure of how quickly the shake effect should evaporate
    private float dampingSpeed = 1f;

    // The initial position of the GameObject
    Vector3 initialPosition;

    void Awake()
    {
        if (transform == null)
        {
            objectTransform = GetComponent(typeof(Transform)) as Transform;
        }
    }

    void OnEnable()
    {
        initialPosition = transform.localPosition;
    }

    // Update is called once per frame
    void Update()
    {
        if (shakeDuration > 0)
        {
            transform.localPosition = initialPosition + Random.insideUnitSphere * shakeMagnitude;

            shakeDuration -= Time.deltaTime * dampingSpeed;
        }
        else
        {
            shakeDuration = 0f;
            transform.localPosition = initialPosition;
        }
    }

    public void TriggerShake()
    {
        shakeDuration = 2.0f;
    }

    public void StopShake()
    {
        shakeDuration = 0;
    }
}
