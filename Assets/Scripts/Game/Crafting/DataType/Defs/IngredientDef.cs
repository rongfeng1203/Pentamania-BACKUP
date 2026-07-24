using UnityEngine;

public abstract class IngredientDef : ScriptableObject
{
    [SerializeField] protected string ingredientId;
    [SerializeField] private int audioType;
    //[SerializeField] protected IngredientProperty[] properties;

    public string GetId()
    {
        return ingredientId;
    }

    public int GetAudioType()
    {
        return audioType;
    }
}


