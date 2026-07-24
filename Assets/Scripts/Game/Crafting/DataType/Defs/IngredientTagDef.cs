using UnityEngine;

[CreateAssetMenu(menuName = "Game/Defs/Tag", fileName = "TagDef")]
public class IngredientTagDef : ScriptableObject
{
    [SerializeField] private string id;
    [SerializeField] private Color color;
    [SerializeField] private Color inverseElementColor;
    [SerializeField] private float colorWeight;
    [SerializeField] private float defaultRangeMin;
    [SerializeField] private float defaultRangeMax;

    public string GetId()
    {
        return id;
    }

    public Color GetColor(bool getInverse)
    {
        return getInverse ? inverseElementColor : color;
    }

    public float GetColorWeight()
    {
        return colorWeight;
    }

    public float GetDefaultMin()
    {
        return defaultRangeMin;
    }

    public float GetDefaultMax()
    {
        return defaultRangeMax;
    }
}