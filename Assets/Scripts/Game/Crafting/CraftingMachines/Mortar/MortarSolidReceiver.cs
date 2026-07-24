using UnityEngine;

[RequireComponent(typeof(Collider))]
public class MortarSolidReceiver : MonoBehaviour
{
    private MortarObject mortar;

    void Awake()
    {
        GetComponent<Collider>().isTrigger = true;
        mortar = GetComponentInParent<MortarObject>();
    }

    void OnTriggerEnter(Collider other)
    {
        var so = other.GetComponent<SolidObject>();
        if (so && so.GetType() == typeof(SolidObject))
            mortar.SolidEntered(so);
    }
    void OnTriggerExit(Collider other)
    {
        var so = other.GetComponent<SolidObject>();
        if (so && so.GetType() == typeof(SolidObject))
            mortar.SolidExited(so);
    }
}