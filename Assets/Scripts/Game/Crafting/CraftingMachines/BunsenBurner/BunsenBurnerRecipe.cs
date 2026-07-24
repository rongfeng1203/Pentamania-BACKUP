using System;
using UnityEngine;

[CreateAssetMenu(menuName = "Game/Recipe/BunsenBurner", fileName = "BunsenBurnerRecipe")]
public class BunsenBurnerRecipe : ScriptableObject
{
    public ItemStack input;
    public ItemStack output;
    public float burnTime = 5f;
}