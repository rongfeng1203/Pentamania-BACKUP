using System;
using UnityEngine;

[CreateAssetMenu(menuName = "Game/Recipe/Mortar", fileName = "MortarRecipe")]
public class MortarRecipe : ScriptableObject
{
    [Serializable]
    public struct ReactantSpec { public ItemDef ingredient; public int amount; }
    
    public ReactantSpec[] reactants;
    public FluidStack product;
}