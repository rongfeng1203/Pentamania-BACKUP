using System;
using UnityEditor;
using UnityEngine;
using UnityEngine.Serialization;

[System.Serializable]
public class IngredientTag
{
    public IngredientTagDef ingredientTagDef;
    public float value;

    public IngredientTag(IngredientTagDef ingredientTagDef, float value)
    {
        this.ingredientTagDef = ingredientTagDef;
        this.value = value;
    }

    public IngredientTag(IngredientTag src)
    {
        ingredientTagDef = src.ingredientTagDef;
        value = src.value;
    }

    public override bool Equals(object obj)
    {
        if (obj is IngredientTag other)
        {
            if (ingredientTagDef == null || other.ingredientTagDef == null)
                return true;
            return ingredientTagDef == other.ingredientTagDef && Mathf.Approximately(this.value, other.value);
        }
        return false;
    }

    /*
    public override int GetHashCode()
    {
        return ingredientTagDef.GetHashCode() ^ value.GetHashCode();
    }
    */

    public override int GetHashCode()
    {
        unchecked
        {
            int hash = 17;
            hash = hash * 31 + (ingredientTagDef != null ? ingredientTagDef.GetHashCode() : 0);
            hash = hash * 31 + value.GetHashCode();
            return hash;
        }
    }

    public bool Equals(IngredientTag tag)
    {
        return tag.ingredientTagDef == ingredientTagDef && tag.value == value;
    }

    public Color GetColor()
    {
        return ingredientTagDef.GetColor(value >= 0);
    }
}

[System.Serializable]
public class IngredientProperty
{
    public string id;
    public float value;

    public IngredientProperty(string id, float value)
    {
        this.id = id;
        this.value = value;
    }
}