using System.Collections.Generic;
using UnityEngine;

public enum TaskCategory
{
    SubmitItem,
    CraftItem
}

[System.Serializable]
public struct ItemRequirement
{
    public FluidDef fluid;
    public int count;
    public List<TagRequirement> tags;
    public float otherTags;
}

[System.Serializable]
public struct Reward
{
    public GameObject prefab;
    public int amount;
}

public abstract class TaskDef : ScriptableObject
{
    public string taskId;
    public string displayName;
    [TextArea] public string description;
    public Reward[] rewards;
    public Sprite icon;

    public abstract TaskCategory Category { get; }
}


[System.Serializable]
public class TagRequirement
{
    public IngredientTagDef tag;
    public float amount;
    public bool more;
}