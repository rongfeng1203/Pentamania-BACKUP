using UnityEngine;

public class PestleObject : MonoBehaviour
{
    [SerializeField] private Vector3 returnPosition;
    [SerializeField] private float returnCheckLevel;

    private void Update()
    {
        if (transform.position.y <= returnCheckLevel)
        {
            transform.position = returnPosition;
            GetComponent<Rigidbody>().linearVelocity = Vector3.zero;
        }
    }
}