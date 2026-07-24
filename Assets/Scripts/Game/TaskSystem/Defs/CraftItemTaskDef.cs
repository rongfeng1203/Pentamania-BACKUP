using UnityEngine;

[CreateAssetMenu(menuName = "Game/Missions/Craft Items", fileName = "CraftItemTask")]
public class CraftItemTaskDef : TaskDef
{
    public ItemRequirement[] products;

    public override TaskCategory Category
    {
        get { return TaskCategory.CraftItem; }
    }
}