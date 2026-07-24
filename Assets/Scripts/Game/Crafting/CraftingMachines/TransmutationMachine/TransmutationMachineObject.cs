using System.Collections.Generic;
using UnityEngine;

public class TransmutationMachineObject : MonoBehaviour
{
    [SerializeField] private TransmutationMachineRecipe[] recipeList;
    [SerializeField] private Transform productSpawnPoint;

    private TransmutationMachine burnerData;

    private void Awake()
    {
        burnerData = new TransmutationMachine(recipeList);
    }

    public void SolidEntered(SolidObject enteredObject)
    {
        burnerData.AddSolid(enteredObject);
    }

    public void SolidExited(SolidObject exitedObject)
    {
        burnerData.RemoveSolid(exitedObject);
    }

    private void Update()
    {
        List<SolidObject> finishedObjects = burnerData.Tick(Time.deltaTime);
        for (int i = 0; i < finishedObjects.Count; i++)
        {
            ConvertSolid(finishedObjects[i]);
        }
    }

    private void ConvertSolid(SolidObject targetObject)
    {
        if (targetObject == null)
        {
            return;
        }

        IngredientStack ingredientInfo = targetObject.GetIngredient();
        if (!(ingredientInfo is ItemStack))
        {
            return;
        }

        ItemStack currentStack = (ItemStack)ingredientInfo;
        if (currentStack.IsEmpty)
        {
            return;
        }

        TransmutationMachineRecipe matchedRecipe;
        bool found = TryGetRecipe(currentStack.Def, out matchedRecipe);
        if (!found)
        {
            return;
        }

        ItemStack newStack = new ItemStack(matchedRecipe.output, currentStack.amount);
        targetObject.SetIngredient(newStack);

        if (productSpawnPoint != null)
        {
            Vector3 offset = UnityEngine.Random.insideUnitSphere * 0.1f;
            targetObject.transform.position = productSpawnPoint.position + offset;
        }
    }

    private bool TryGetRecipe(ItemDef inputDefinition, out TransmutationMachineRecipe matchedRecipe)
    {
        for (int i = 0; i < recipeList.Length; i++)
        {
            TransmutationMachineRecipe currentRecipe = recipeList[i];
            if (currentRecipe != null && currentRecipe.input == inputDefinition)
            {
                matchedRecipe = currentRecipe;
                return true;
            }
        }

        matchedRecipe = null;
        return false;
    }
}