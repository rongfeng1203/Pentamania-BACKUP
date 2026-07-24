using System;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

public class ItemDeletion : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        //Debug.Log("111rytu");
        if (other.TryGetComponent<SolidObject>(out var interactable))
        {
            Debug.Log("Deleted item");
            if (interactable.gameObject != gameObject)
            {
                Destroy(interactable.gameObject);
            }
        }
    }
}