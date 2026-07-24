using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

[RequireComponent(typeof(XRGrabInteractable))]
public class ObjectPickUp : MonoBehaviour
{
    private XRGrabInteractable grabInteractable;

    void Awake()
    {
        grabInteractable = GetComponent<XRGrabInteractable>();
    }

    void OnEnable()
    {
        grabInteractable.selectEntered.AddListener(OnGrabbed);
        grabInteractable.selectExited.AddListener(OnReleased);
    }

    void OnDisable()
    {
        grabInteractable.selectEntered.RemoveListener(OnGrabbed);
        grabInteractable.selectExited.RemoveListener(OnReleased);
    }

    private void OnGrabbed(SelectEnterEventArgs args)
    {
        var f = GetComponent<FluidContainer>();
        if (f != null && !f.GetRealFluidIngredient().IsEmpty)
        {
            AudioManager.instance.PickupIngredient(7, gameObject);
            Debug.Log("Picked up the object: " + 7);
        }
        else
        {
            AudioManager.instance.PickupIngredient(GetComponent<PassableIngredientObject>());
            Debug.Log("Picked up the object: " + GetComponent<PassableIngredientObject>().GetAudioType());
        }
    }

    private void OnReleased(SelectExitEventArgs args)
    {
        var f = GetComponent<FluidContainer>();
        if (f != null && !f.GetRealFluidIngredient().IsEmpty)
        {
            AudioManager.instance.PickupIngredient(7, gameObject);
            Debug.Log("Dropped the object: " + 7);
        }
        else
        {
            AudioManager.instance.PickupIngredient(GetComponent<PassableIngredientObject>());
            Debug.Log("Dropped the object: " + GetComponent<PassableIngredientObject>().GetAudioType());
        }
        
    }
}