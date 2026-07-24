using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider))]
public class CauldronLiquidReceiver : MonoBehaviour
{
    private CauldronObject cauldron;
    private readonly HashSet<FluidContainer> sources = new HashSet<FluidContainer>();
    
    private void Awake()
    {
        GetComponent<Collider>().isTrigger = true;
        cauldron = GetComponentInParent<CauldronObject>();
    }
    
    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("[Cauldron Liquid receiver] Received liquid");
        var src = other.GetComponent<FluidContainer>();
        if (src) sources.Add(src);
    }
    private void OnTriggerExit(Collider other)
    {
        Debug.Log("[Cauldron Liquid receiver] Stopped Receiving liquid");
        var src = other.GetComponent<FluidContainer>();
        if (src)
        {
            sources.Remove(src);
            src.StopPour();
        }
    }

    private void FixedUpdate()
    {
        if (sources.Count == 0) return;

        var toRemove = new List<FluidContainer>();

        foreach (var src in sources)
        {
            if (src == null) { toRemove.Add(src); continue; }
            
            FluidStack drained = src.TryPour();
            if (drained.IsEmpty) continue;
            
            float remain = cauldron.ReceiveLiquid(drained);

            drained.volume = remain;
            
            if (remain > 0f)
                src.Fill(drained);
        }

        foreach (var s in toRemove) sources.Remove(s);
    }
}