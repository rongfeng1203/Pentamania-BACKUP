using UnityEngine;
 
[RequireComponent(typeof(Collider))]
public class CauldronScoopReceiver : MonoBehaviour
{
    private CauldronObject cauldron;
    private void Awake()
    {
        GetComponent<Collider>().isTrigger = true;
        cauldron = GetComponentInParent<CauldronObject>();
    }
    private void OnTriggerEnter(Collider other)
    {
        var c = other.GetComponent<IFluidContainer>();
        if (c == null)
            return;
        AudioManager.instance.PlaySound("cauldron_pickup", gameObject);
        cauldron.TryScoopLiquid(c);
    }
}