using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Serialization;

/// <summary>
/// A recipe defines an interaction between ingredients (reactants) which result in another ingredient for a specific machine.
/// They are primarily defined by JSON files.
/// </summary>
[Serializable]
[CreateAssetMenu(menuName = "Game/Recipe/Cauldron")]
public class CauldronRecipe : ScriptableObject
{
    // stores which ingredients must be present to cause the reaction.
    //public IngredientRequirement[] requiredReactants;
    // stores which ingredients are to be created after the reaction occurs.
    // for json conversion, this is a string array of the paths to IngredientObjects used to get the Ingredients this Recipe produces.

    //public Requirement[] reactants;
    public IngredientRequirement[] requirements;
    public ProductSpec[] products;

    public bool CheckForRecipeMatch(Dictionary<IngredientTagDef, double> tagMap)
    {
        string debugOut = "[Cauldron] RECIPE MATCH LOGIC\n\n";

        // Compare tags to myself
        foreach (IngredientTagDef tag in tagMap.Keys)
        {
            foreach (IngredientRequirement requirement in requirements)
            {
                debugOut += $"Comparing ingredient tag {tag.GetId()} with requirement tag {requirement.ingredientTag.GetId()}: ";
                if (requirement.ingredientTag == tag)
                {
                    debugOut += "match";
                    if (tagMap[tag] < requirement.min || tagMap[tag] > requirement.max)
                    {
                        debugOut += $" FAIL. \nTAG VALUE INCORRECTS.\n Ing: {tag.GetId()} with val {tagMap[tag]} did not fit min {requirement.min} max {requirement.max}";
                        Debug.Log(debugOut);
                        return false;
                    }
                }
                else
                {
                    debugOut += "no match, checking default";
                    if (tagMap[tag] < tag.GetDefaultMin() || tagMap[tag] > tag.GetDefaultMax())
                    {
                        debugOut += $" FAIL. \nTAG OUTSIDE DEFAULT.\n Ing: {tag.GetId()} with val {tagMap[tag]} did not fit default min {tag.GetDefaultMin()} default max {tag.GetDefaultMax()}";
                        Debug.Log(debugOut);
                        return false;
                    }
                }
                debugOut += "...success. \n";
            }
            debugOut += "Moving to next ingredient tag.\n\n";
        }

        debugOut += "! Survived elimination round.\n\nChecking that all requirements have a match in the tags.\n";

        // make sure all requirements were checked
        foreach (IngredientRequirement requirement in requirements)
        {
            debugOut += $"Checking requirement {requirement.ingredientTag.GetId()}...";

            double tagVal;
            if (!tagMap.Keys.Contains(requirement.ingredientTag))
            {
                tagVal = 0;
            }
            else
            {
                tagVal = tagMap[requirement.ingredientTag];
            }

            if (tagVal < requirement.min || tagVal > requirement.max)
            {
                debugOut += $"FAIL. TagVal of {tagVal} not within min {requirement.min} max {requirement.max}.\nFAIL!";
                Debug.Log(debugOut);
                return false;
            }
            debugOut += $"Success. TagVal of {tagVal} WITHIN min {requirement.min} max {requirement.max}.\n";
        }

        debugOut += "PASS ALL!!";
        Debug.Log(debugOut);

        return true;
    }

    public bool CheckForRecipeMatch(Dictionary<IngredientTagDef, double> tagMap, out bool violatePower)
    {
        string debugOut = "[Cauldron] RECIPE MATCH LOGIC\n\n";
        violatePower = false;
        // Compare tags to myself
        foreach (IngredientTagDef tag in tagMap.Keys)
        {
            bool hasFoundMatch = false;
            foreach (IngredientRequirement requirement in requirements)
            {
                debugOut += $"Comparing ingredient tag {tag.GetId()} with requirement tag {requirement.ingredientTag.GetId()}: ";
                if (requirement.ingredientTag == tag)
                {
                    debugOut += "match";
                    if (tagMap[tag] < requirement.min || tagMap[tag] > requirement.max)
                    {
                        debugOut += $" FAIL. \nTAG VALUE INCORRECTS.\n Ing: {tag.GetId()} with val {tagMap[tag]} did not fit min {requirement.min} max {requirement.max}";
                        Debug.Log(debugOut);
                        if (requirement.isMain) violatePower = true;
                        return false;
                    }
                    hasFoundMatch = true;
                    debugOut += "success. \n";
                    break;
                }
                debugOut += "no match.\n";
            }

            if (!hasFoundMatch)
            {
                debugOut += "No match at all, checking default";
                if (tagMap[tag] < tag.GetDefaultMin() || tagMap[tag] > tag.GetDefaultMax())
                {
                    debugOut += $" FAIL. \nTAG OUTSIDE DEFAULT.\n Ing: {tag.GetId()} with val {tagMap[tag]} did not fit default min {tag.GetDefaultMin()} default max {tag.GetDefaultMax()}";
                    Debug.Log(debugOut);
                    return false;
                }
                debugOut += "...success.";
            }

            debugOut += "Moving to next ingredient tag.\n\n";
        }

        debugOut += "! Survived elimination round.\n\nChecking that all requirements have a match in the tags.\n";

        // make sure all requirements were checked
        foreach (IngredientRequirement requirement in requirements)
        {
            debugOut += $"Checking requirement {requirement.ingredientTag.GetId()}...";

            double tagVal;
            if (!tagMap.Keys.Contains(requirement.ingredientTag))
            {
                tagVal = 0;
            }
            else
            {
                tagVal = tagMap[requirement.ingredientTag];
            }

            if (tagVal < requirement.min || tagVal > requirement.max)
            {
                debugOut += $"FAIL. TagVal of {tagVal} not within min {requirement.min} max {requirement.max}.\nFAIL!";
                Debug.Log(debugOut);
                return false;
            }
            debugOut += $"Success. TagVal of {tagVal} WITHIN min {requirement.min} max {requirement.max}.\n";
        }

        debugOut += "PASS ALL!!";
        Debug.Log(debugOut);

        return true;
    }
}

[Serializable]
public struct IngredientRequirement
{
    [FormerlySerializedAs("Tag")] public IngredientTagDef ingredientTag;
    public float min;
    public float max;
    public bool isMain;
}

[Serializable]
public struct ProductSpec
{
    public ItemStack itemProduct;
    public FluidStack fluidProduct;
    //public IngredientDef ingredient;
    public float amount;
}

[Serializable]
public struct Requirement {
    public IngredientDef ingredient;
    public float minAmount;
    public float maxAmount;
    public bool proportional;
}
