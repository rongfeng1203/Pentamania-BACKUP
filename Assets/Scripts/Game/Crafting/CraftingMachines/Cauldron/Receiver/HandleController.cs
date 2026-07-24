using UnityEngine;

public class HandleController : MonoBehaviour
{
    public float Angle { get; private set; }

    void Update()
    {
        Angle = transform.localRotation.y;
    }
}