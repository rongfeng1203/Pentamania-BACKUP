using UnityEngine;

[CreateAssetMenu(menuName = "Game/Defs/Fluid", fileName = "FluidDef")]
public class FluidDef : IngredientDef
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