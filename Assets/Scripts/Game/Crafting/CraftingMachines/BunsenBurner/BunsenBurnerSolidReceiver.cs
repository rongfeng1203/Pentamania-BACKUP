using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider))]
public class BunsenBurnerSolidReceiver : MonoBehaviour
{
    private BunsenBurnerObject burner;

    private void Awake()
    {
        var col = GetComponent<Collider>();
        col.isTrigger = true;
        burner = GetComponentInParent<BunsenBurnerObject>();
    }

    private void OnTriggerEnter(Collider other)
    {
        var so = other.GetComponent<SolidObject>();
        if (so && so.GetType() == typeof(SolidObject))
            burner.SolidEntered(so);
    }

    private void OnTriggerExit(Collider other)
    {
        var so = other.GetComponent<SolidObject>();
        if (so && so.GetType() == typeof(SolidObject))
            burner.SolidExited(so);
    }
}