using System.Collections.Generic;
using System.Diagnostics;
using Debug = UnityEngine.Debug;

public class CrystalBall
{
    private readonly List<CrystalBallRecipe> recipes;

    public CrystalBall(IEnumerable<CrystalBallRecipe> recipes)
    {
        this.recipes = new List<CrystalBallRecipe>(recipes);
    }

    public bool TryConversion(IngredientStack ingredient, out IngredientStack product)
    {
        Debug.Log("CRYSTAL BALL TRY CONVERSION");
        Debug.Log(ingredient.GetType().ToString());
        if (ingredient.GetType() == typeof(FluidStack))
        {
            Debug.Log("Found liquid ingredient, running alt");
            List<IngredientTag> tags = new();

            foreach (IngredientTag tag in ingredient.GetTags())
            {
                tags.Add(new IngredientTag(tag.ingredientTagDef, tag.value * -1));
                Debug.Log("Crystal Ball: Tag in process: " + tag.ingredientTagDef + " with val: " + tag.value);
            }

            FluidStack fluidStack = (FluidStack)ingredient;
            product = new FluidStack((FluidDef)ingredient.GetAbstractDef(), fluidStack.volume, tags);

            // DEBUG
            foreach (IngredientTag tag in product.GetTags())
            {
                Debug.Log("Crystal Ball Result ID:" + tag.ingredientTagDef.GetId() + " with quant " + tag.value);
            }

            return true;
        }

        Debug.Log("Checking all recipes");
        foreach (var recipe in recipes)
        {


            if (MatchRecipe(recipe, ingredient))
            {
                // please send help
                product = recipe.GetProduct();
                return true;
            }

            if (MatchReverseRecipe(recipe, ingredient))
            {
                product = recipe.GetReactant();
                return true;
            }
        }

        product = null;
        return false;
    }

    private bool MatchRecipe(CrystalBallRecipe recipe, IngredientStack ingredient)
    {
        if (ingredient == null || recipe == null)
        {
            return false;
        }
        return ingredient.GetAbstractDef().GetId().Equals(recipe.GetReactant().GetAbstractDef().GetId());
    }

    private bool MatchReverseRecipe(CrystalBallRecipe recipe, IngredientStack ingredient)
    {
        if (ingredient == null || recipe == null)
        {
            return false;
        }
        return ingredient.GetAbstractDef().GetId().Equals(recipe.GetProduct().GetAbstractDef().GetId());
    }
}
