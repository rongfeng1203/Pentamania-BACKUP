
using System;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(menuName = "Game/Recipe/CrystalBall")]
public class CrystalBallRecipe : ScriptableObject
{
    [SerializeField] private IngredientDef reactant;
    [SerializeField] protected IngredientDef product;

    void Awake()
    {
        if (reactant.GetType() != product.GetType())
        {
            throw new ArgumentException("A crystal ball cannot convert between liquids and solids!");
        }
    }

    public IngredientStack GetReactant()
    {
        if (reactant.GetType() == typeof(ItemDef))
        {
            ItemDef reactantDef = (ItemDef)reactant;
            var temp = new ItemStack(reactantDef, 1);
            temp.tags = reactantDef.GetPrefab().GetComponent<SolidObject>().GetIngredient().GetTags().ToList();
            return temp;
        }
        else if (reactant.GetType() == typeof(FluidDef))
        {
            return new FluidStack((FluidDef)reactant, 1);
        }
        throw new Exception("A crystal ball recipe was found to contain a reactant that was neither a fluid nor an item.");
    }

    public IngredientStack GetProduct()
    {
        if (product.GetType() == typeof(ItemDef))
        {
            ItemDef productDef = (ItemDef)product;
            var temp = new ItemStack(productDef, 1);
            temp.tags = productDef.GetPrefab().GetComponent<SolidObject>().GetIngredient().GetTags().ToList();
            return temp;
        }
        else if (product.GetType() == typeof(FluidDef))
        {
            return new FluidStack((FluidDef)product, 1);
        }
        throw new Exception("A crystal ball recipe was found to contain a product that was neither a fluid nor an item.");
    }

    public virtual bool TryConversion(IngredientStack ingredient, out IngredientStack product)
    {
        ingredient = null;
        product = null;
        return false;
    } 
}