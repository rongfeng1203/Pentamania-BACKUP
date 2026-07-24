

using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.VFX;

public class CrystalBallObject : MonoBehaviour
{
    [SerializeField] private CrystalBallRecipe[] recipeList;
    private bool canConvert = true;

    private CrystalBall ballData;

    public VisualEffect craftingEffect;

    private void Awake()
    {
        ballData = new CrystalBall(recipeList);
        AudioManager.instance.PlaySound("crystal_ball_ambience", gameObject);
    }

    public void IngredientObjectEntered(PassableIngredientObject ingredientObject)
    {
        if (!canConvert)
        {
            return;
        }

        if (ingredientObject.GetType() == typeof(FluidContainer))
        {
            FluidContainer fc = (FluidContainer)ingredientObject;
            
            string debugOut2 = "Beginning crystal ball conversion. Printing all tags.";
            foreach (IngredientTag tag in fc.GetFluidIngredient().GetTags())
            {
                debugOut2 += $"Tag: {tag.ingredientTagDef} with value: {tag.value}";
            }
            Debug.Log(debugOut2);

            if (ballData.TryConversion(fc.GetFluidIngredient(), out IngredientStack potionIngredient))
            {
                canConvert = false;

                fc.SetFluidIngredient(potionIngredient);
                FluidStack fs = (FluidStack)fc.GetFluidIngredient();
                fs.tags = potionIngredient.GetTags().ToList();

                string debugOut = "Should've finished crystal ball conversion. Printing all tags.";
                foreach (IngredientTag tag in fc.GetFluidIngredient().GetTags())
                {
                    debugOut += $"Tag: {tag.ingredientTagDef} with value: {tag.value}\n";
                }
                Debug.Log(debugOut);


                // CREATE PARTICLES HERE
                if (craftingEffect != null) { craftingEffect.Play(); }
                AudioManager.instance.PlaySound("crystal_ball_usage", gameObject);
            }
            return;
        }
        
        string debugOut1 = "Beginning crystal ball conversion. Printing all tags.";
        foreach (IngredientTag tag in ingredientObject.GetIngredient().GetTags())
        {
            debugOut1 += $"Tag: {tag.ingredientTagDef} with value: {tag.value}";
        }
        Debug.Log(debugOut1);

        if (ballData.TryConversion(ingredientObject.GetIngredient(), out IngredientStack ingredient))
        {
            canConvert = false;

            ingredientObject.SetIngredient(ingredient);
            ingredientObject.SetTags(ingredient.GetTags().ToList());

            string debugOut = "Should've finished crystal ball conversion. Printing all tags.";
            foreach (IngredientTag tag in ingredientObject.GetIngredient().GetTags())
            {
                debugOut += $"Tag: {tag.ingredientTagDef} with value: {tag.value}\n";
            }
            Debug.Log(debugOut);


            // CREATE PARTICLES HERE
            if (craftingEffect != null) { craftingEffect.Play(); }
            AudioManager.instance.PlaySound("crystal_ball_usage", gameObject);
        }
    }

    public void IngredientObjectExit() {
        canConvert = true;
    }
}