using UnityEngine;

[RequireComponent(typeof(Collider))]
public class TransmutationMachineObjectReceiver : MonoBehaviour
{
    private TransmutationMachineObject transmutationMachine;

    private void Awake()
    {
        GetComponent<Collider>().isTrigger = true;
        transmutationMachine = GetComponentInParent<TransmutationMachineObject>();
    }

    private void OnTriggerEnter(Collider other)
    {
        PassableIngredientObject solidObject = other.GetComponent<PassableIngredientObject>();
        if (!solidObject)
            return;
        if (solidObject.GetType() != typeof(SolidObject))
            return;
        
        //transmutationMachine.SolidEntered(solidObject);
    }

    private void OnTriggerExit(Collider other)
    {
        PassableIngredientObject solidObject = other.GetComponent<PassableIngredientObject>();
        if (!solidObject)
            return;
        if (solidObject.GetType() != typeof(SolidObject))
            return;
        
        //transmutationMachine.SolidExited(solidObject);
    }
}