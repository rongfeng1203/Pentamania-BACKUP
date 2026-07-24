using System;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider))]
public class MortarPestleReceiver : MonoBehaviour
{
    private MortarObject mortar;

    private void Awake()
    {
        GetComponent<Collider>().isTrigger = true;
        mortar = GetComponentInParent<MortarObject>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.GetComponent<PestleObject>() == null)
            return;
        if (!mortar)
            return;
        mortar.PestleHit();
    }
}