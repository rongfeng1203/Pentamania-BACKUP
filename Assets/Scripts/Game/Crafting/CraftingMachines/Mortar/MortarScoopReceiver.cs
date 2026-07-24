using UnityEngine;

[RequireComponent(typeof(Collider))]
public class MortarScoopReceiver : MonoBehaviour
{
    private MortarObject mortar;
    void Awake()
    {
        GetComponent<Collider>().isTrigger = true; 
        mortar = GetComponentInParent<MortarObject>();
    }

    void OnTriggerEnter(Collider other)
    {
        var cont = other.GetComponent<IFluidContainer>();
        if (cont != null)
            mortar.TryScoop(cont);
    }
}