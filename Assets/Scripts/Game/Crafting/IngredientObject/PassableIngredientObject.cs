using System.Collections.Generic;
using UnityEngine;

public abstract class PassableIngredientObject : MonoBehaviour
{
    public abstract IngredientStack GetIngredient();
    public abstract void SetIngredient(IngredientStack ingredient);


    public int GetAudioType()
    {
        return GetIngredient().GetAbstractDef().GetAudioType();
    }

    public abstract void SetTags(List<IngredientTag> tags);
}
 