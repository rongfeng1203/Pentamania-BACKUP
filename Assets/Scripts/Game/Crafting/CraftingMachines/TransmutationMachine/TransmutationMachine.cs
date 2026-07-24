using System.Collections.Generic;
using UnityEngine;

public class TransmutationMachine
{
    private readonly List<BunsenEntry> trackedEntries = new List<BunsenEntry>();
    private readonly Dictionary<ItemDef, TransmutationMachineRecipe> recipeDictionary;

    public TransmutationMachine(IEnumerable<TransmutationMachineRecipe> recipeCollection)
    {
        recipeDictionary = new Dictionary<ItemDef, TransmutationMachineRecipe>();
        foreach (TransmutationMachineRecipe currentRecipe in recipeCollection)
        {
            if (currentRecipe != null && currentRecipe.input != null && currentRecipe.output != null)
                recipeDictionary[currentRecipe.input] = currentRecipe;
        }
    }

    public void AddSolid(SolidObject newObject)
    {
        if (newObject == null)
            return;

        IngredientStack ingredientInfo = newObject.GetIngredient();
        if (!(ingredientInfo is ItemStack))
            return;

        ItemStack objectStack = (ItemStack)ingredientInfo;
        if (objectStack.IsEmpty)
            return;

        for (int i = 0; i < trackedEntries.Count; i++)
        {
            if (trackedEntries[i].trackedSolid == newObject)
                return;
        }

        trackedEntries.Add(new BunsenEntry(newObject, 0f));
    }

    public void RemoveSolid(SolidObject leavingObject)
    {
        for (int i = trackedEntries.Count - 1; i >= 0; i--)
        {
            if (trackedEntries[i].trackedSolid == leavingObject)
                trackedEntries.RemoveAt(i);
        }
    }

    public List<SolidObject> Tick(float deltaTime)
    {
        List<SolidObject> finishedObjects = new List<SolidObject>();

        for (int i = trackedEntries.Count - 1; i >= 0; i--)
        {
            BunsenEntry entry = trackedEntries[i];

            if (entry.trackedSolid == null)
            {
                trackedEntries.RemoveAt(i);
                continue;
            }

            entry.timer += deltaTime;
            trackedEntries[i] = entry;

            IngredientStack ingredientInfo = entry.trackedSolid.GetIngredient();
            if (!(ingredientInfo is ItemStack))
                continue;

            ItemStack currentStack = (ItemStack)ingredientInfo;
            if (currentStack.IsEmpty)
                continue;

            TransmutationMachineRecipe matchingRecipe;
            if (!recipeDictionary.TryGetValue(currentStack.Def, out matchingRecipe))
                continue;
            

            if (entry.timer >= matchingRecipe.cookTime)
            {
                finishedObjects.Add(entry.trackedSolid);
                trackedEntries.RemoveAt(i);
            }
        }

        return finishedObjects;
    }

    private struct BunsenEntry
    {
        public SolidObject trackedSolid;
        public float timer;

        public BunsenEntry(SolidObject solid, float startTimer)
        {
            trackedSolid = solid;
            timer = startTimer;
        }
    }
}