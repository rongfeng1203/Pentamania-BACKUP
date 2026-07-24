using UnityEngine;

[CreateAssetMenu(menuName = "Game/Defs/Custom/Potion", fileName = "PotionDef")]
public class Potion : FluidDef
{
    [SerializeField] private Material material;
    
    public Material GetMaterial()
    {
        return material;
    }

/*
    public IngredientProperty[] GetProperties()
    {
        return properties;
    }
    */
}