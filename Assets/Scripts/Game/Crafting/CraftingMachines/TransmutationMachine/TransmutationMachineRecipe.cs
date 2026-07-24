using System;
using UnityEngine;

[Serializable]
[CreateAssetMenu(menuName = "Game/Recipe/TransmutationMachine", fileName = "TransmutationMachineRecipe")]
public class TransmutationMachineRecipe : ScriptableObject
{
    public ItemDef input;
    public ItemDef output;
    public float cookTime = 3f;
}