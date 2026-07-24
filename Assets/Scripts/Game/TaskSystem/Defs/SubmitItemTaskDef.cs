using UnityEngine;

[CreateAssetMenu(menuName = "Game/Missions/Submit Items", fileName = "SubmitItemTask")]
public class SubmitItemTaskDef : TaskDef
{
    public ItemRequirement[] requirements;

    public override TaskCategory Category
    {
        get { return TaskCategory.SubmitItem; }
    }
}
