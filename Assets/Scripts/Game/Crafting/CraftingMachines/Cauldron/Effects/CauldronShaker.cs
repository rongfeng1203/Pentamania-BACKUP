using UnityEngine;

public class CauldronShaker : MonoBehaviour
{
    public float positionShakeStrength = 0.1f;
    public float positionShakeSpeed = 20f;
    
    public float rotationShakeStrength = 5f;
    public float rotationShakeSpeed = 20f;

    private Vector3 initialPosition;
    private Quaternion initialRotation;

    public void Start()
    {
        initialPosition = transform.localPosition;
        initialRotation = transform.localRotation;
    }

    void Update()
    {
        Vector3 posOffset = new Vector3(
            Mathf.PerlinNoise(Time.time * positionShakeSpeed, 0f) - 0.5f,
            Mathf.PerlinNoise(0f, Time.time * positionShakeSpeed) - 0.5f,
            Mathf.PerlinNoise(Time.time * positionShakeSpeed, Time.time * positionShakeSpeed) - 0.5f
        ) * positionShakeStrength;

        Vector3 rotOffset = new Vector3(
            Mathf.PerlinNoise(Time.time * rotationShakeSpeed, 1f) - 0.5f,
            Mathf.PerlinNoise(1f, Time.time * rotationShakeSpeed) - 0.5f,
            Mathf.PerlinNoise(Time.time * rotationShakeSpeed, Time.time * rotationShakeSpeed) - 0.5f
        ) * rotationShakeStrength;

        transform.localPosition = initialPosition + posOffset;
        transform.localRotation = Quaternion.Euler(rotOffset) * initialRotation;
    }
}