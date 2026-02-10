using UnityEngine;

public class PendulumSwing : MonoBehaviour
{
    [SerializeField] private float length = 2f;
    [SerializeField] private float maxAngle = 30f;
    [SerializeField] private float timeOffset = 0f;
    [SerializeField] private bool swingAroundZ = true;

    [SerializeField] private bool sign;

    private float angularFrequency;

    void Start()
    {
        angularFrequency = Mathf.Sqrt(9.81f / length);
    }

    void Update()
    {
        float time = Time.time + timeOffset;
        float angle = maxAngle * Mathf.Cos(angularFrequency * time);

        Vector3 euler = transform.localEulerAngles;
        if (swingAroundZ)
            euler.z = angle;
        else
            euler.x = angle;

        transform.localEulerAngles = euler;
        
        switch (angle)
        {
            case > 0 when !sign:
                OnSignChange();
                sign = true;
                break;
            case < 0 when sign:
                OnSignChange();
                sign = false;
                break;
        }
        
    }

    void OnSignChange()
    {
        AudioManager.instance.PlaySound("clock_tick", gameObject);
    }
}