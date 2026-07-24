using UnityEngine;

[RequireComponent(typeof(Collider))]
public class TaskSubmitZone : MonoBehaviour
{
    private void Awake()
    {
        Collider col = GetComponent<Collider>();
        col.isTrigger = true;
    }

    private void OnTriggerEnter(Collider other)
    {
        FluidContainer solid = other.GetComponent<FluidContainer>();
        if (solid == null) return;
        
        IngredientStack info = solid.GetFluidIngredient();
        if (!(info is FluidStack)) return;
        
        FluidStack stack = (FluidStack)info;
        IngredientStack itemInfo = solid.GetIngredient();
        ItemStack itemStack = (ItemStack)itemInfo;
        if (stack.IsEmpty) return;
        
        TaskManager manager = TaskManager.Instance;
        if (manager == null) return;
        
        bool needed = manager.NeedThisItem(stack.Def);
        if (!needed)
        {
            Debug.Log("[Task System] " + stack.Def.GetId() + " is not needed");
            solid.GetComponent<Rigidbody>().AddForce(new Vector3(0, 100, 0), ForceMode.Impulse);
            return;
        }

        AudioManager.instance.PlaySound("magic_circle_accept", gameObject);
        Debug.Log("[Task System] Submited " + stack.Def.GetId() + " x " + stack.volume);
        manager.SubmitItem(stack, itemStack.amount);
        Destroy(solid.gameObject);
    }
}