using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Game.Utilities;

[DefaultExecutionOrder(-100)]
public class TaskManager : MonoBehaviour
{

    public static TaskManager Instance;

    [SerializeField] private Transform rewardSpawnPoint;
    //[SerializeField] private TaskSubmitZone submitZone;

    private readonly Dictionary<string, SubmitProgressData> submitMap =
        new Dictionary<string, SubmitProgressData>();
    
    private readonly List<TaskDef> activeTaskList = new List<TaskDef>();
    private readonly List<TaskDef> finishedTaskList = new List<TaskDef>();

    public int taskFinished;

    public List<Animator> tentacles;

    public event Action<TaskDef, float> ProgressChanged;
    public event Action<TaskDef> TaskCompleted;
    public event Action TaskAssigned;

    public float taskWaitTime;

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    public IReadOnlyList<TaskDef> ActiveTasks
    {
        get { return activeTaskList; }
    }

    public IReadOnlyList<TaskDef> FinishedTasks
    {
        get { return finishedTaskList; }
    }

    public void PushTask(TaskDef task)
    {
        Debug.Log("[Task System] Try pushing the new task");
        if (submitMap.ContainsKey(task.taskId))
        {
            Debug.Log("[Task System] Contains ID");
            return;
        }

        SubmitProgressData progressData = new SubmitProgressData(task);
        submitMap.Add(task.taskId, progressData);
        activeTaskList.Add(task);
        if (TaskAssigned != null)
            TaskAssigned();

        Debug.Log("[Task System] Task assigned: " + task.displayName);
        if (ProgressChanged != null)
        {
            ProgressChanged(task, 0.0f);
        }
    }

    public float GetRatio(TaskDef task)
    {
        SubmitProgressData dataValue;
        if (!submitMap.TryGetValue(task.taskId, out dataValue))
        {
            return 0.0f;
        }
        return dataValue.GetRatio();
    }

    public void ResetAllProgress()
    {
        submitMap.Clear();
        activeTaskList.Clear();
        finishedTaskList.Clear();
        Debug.LogWarning("[Task System] Task system reset");
    }
    
    public void SubmitItem(FluidStack item, int amount)
    {
        FluidDef fluidDefinition = item.Def;
        for (int i = activeTaskList.Count - 1; i >= 0; i--)
        {
            TaskDef genericTask = activeTaskList[i];
            if (genericTask.Category != TaskCategory.SubmitItem)
            {
                continue;
            }

            SubmitItemTaskDef submitTask = (SubmitItemTaskDef)genericTask;
            SubmitProgressData progressData = submitMap[submitTask.taskId];

            bool anyMatched = false;

            for (int r = 0; r < submitTask.requirements.Length; r++)
            {
                ItemRequirement requirement = submitTask.requirements[r];
                if (requirement.fluid != fluidDefinition)
                {
                    continue;
                }

                var tagList = item.tags;

                int index = 0;
                foreach (var tagRequirement in requirement.tags)
                {
                    foreach (var tag in tagList)
                    {
                        if (tag.ingredientTagDef == tagRequirement.tag)
                        {
                            if (tagRequirement.more && tag.value < tagRequirement.amount)
                            {
                                Debug.Log("[Task System] Tag not matched of index: " + index);
                                return;
                            }
                            else if (!tagRequirement.more && tag.value > tagRequirement.amount)
                            {
                                Debug.Log("[Task System] Tag not matched of index: " + index);
                                return;
                            }

                            tagList.Remove(tag);
                            break;
                        }
                    }

                    index++;
                }

                foreach (var tag in tagList)
                {
                    if (tag.value > requirement.otherTags)
                    {
                        return;
                    }
                }

                int before = progressData.currentAmounts[r];
                int target = requirement.count;
                int after = before + amount;
                if (after > target)
                {
                    after = target;
                }
                progressData.currentAmounts[r] = after;
                anyMatched = true;

                Debug.Log("[Task System] Submitted: " + requirement.fluid.GetId() +
                          " x " + amount + ", progress " + after + "/" + target);
                //submitZone
            }

            if (anyMatched)
            {
                CheckSubmitCompletion(submitTask, progressData);
            }
        }
    }

    private void CheckSubmitCompletion(SubmitItemTaskDef task, SubmitProgressData progressData)
    {
        float ratio = progressData.GetRatio();
        if (ProgressChanged != null)
        {
            ProgressChanged(task, ratio);
        }

        if (ratio >= 1.0f)
        {
            MarkTaskFinished(task);
        }
    }
    /*
    public void CraftItemProduced(ItemDef itemDefinition, int amount)
    {
        for (int i = activeTaskList.Count - 1; i >= 0; i--)
        {
            TaskDef genericTask = activeTaskList[i];
            if (genericTask.Category != TaskCategory.CraftItem)
            {
                continue;
            }

            CraftItemTaskDef craftTask = (CraftItemTaskDef)genericTask;
            SubmitProgressData progressData = submitMap[craftTask.taskId];

            bool matched = false;

            for (int p = 0; p < craftTask.products.Length; p++)
            {
                ItemRequirement requirement = craftTask.products[p];
                if (requirement.item != itemDefinition)
                {
                    continue;
                }

                int before = progressData.currentAmounts[p];
                int target = requirement.count;
                int after = before + amount;
                if (after > target)
                {
                    after = target;
                }
                progressData.currentAmounts[p] = after;
                matched = true;

                Debug.Log("[Task System] Crafted: " + requirement.item.GetId() +
                          " x " + amount + ", progress " + after + "/" + target);
            }

            if (matched)
            {
                float ratio = progressData.GetRatio();
                if (ProgressChanged != null)
                {
                    ProgressChanged(craftTask, ratio);
                }
                if (ratio >= 1.0f)
                {
                    MarkTaskFinished(craftTask);
                }
            }
        }
    }
    */
    private void MarkTaskFinished(TaskDef task)
    {
        activeTaskList.Remove(task);
        finishedTaskList.Add(task);

        SpawnTentacle(taskFinished);
        AudioManager.instance.PlaySound("cthulu_tentacle_spawn", gameObject);
        taskFinished ++;
        Debug.Log("[Task System] Task completed: " + task.displayName);
        SpawnRewards(task);

        if (TaskCompleted != null)
        {
            TaskCompleted(task);
        }
        
        if (TaskAssigner.Instance != null)
        {
            StartCoroutine(DelayedAssignNextTask());
        }
    }
    
    private IEnumerator DelayedAssignNextTask()
    {
        Debug.Log("[Task System] Waiting 10s before assigning next task...");
        yield return new WaitForSeconds(taskWaitTime);
    
        if (TaskAssigner.Instance != null)
        {
            Debug.Log("[Task System] Assigning next task now");
            TaskAssigner.Instance.TryAssignNext();
        }
    }

    private void SpawnTentacle(int index)
    {
        tentacles[index].enabled = true;
    }

    private void SpawnRewards(TaskDef task)
    {
        for (int i = 0; i < task.rewards.Length; i++)
        {
            Reward reward = task.rewards[i];
            for (int c = 0; c < Mathf.Max(1, reward.amount); c++)
            {
                Instantiate(reward.prefab, rewardSpawnPoint.position, Quaternion.identity);
            }
        }
        Debug.Log("[Task System] Reward spawned" + task.rewards.Length);
    }
    
    [Serializable]
    private class SubmitProgressData
    {
        public int[] currentAmounts;
        public int[] requiredAmounts;

        public SubmitProgressData(TaskDef task)
        {
            if (task.Category == TaskCategory.SubmitItem)
            {
                SubmitItemTaskDef t = (SubmitItemTaskDef)task;
                int len = t.requirements.Length;
                currentAmounts = new int[len];
                requiredAmounts = new int[len];
                for (int i = 0; i < len; i++)
                {
                    requiredAmounts[i] = t.requirements[i].count;
                }
            }
            else
            {
                CraftItemTaskDef t = (CraftItemTaskDef)task;
                int len = t.products.Length;
                currentAmounts = new int[len];
                requiredAmounts = new int[len];
                for (int i = 0; i < len; i++)
                {
                    requiredAmounts[i] = t.products[i].count;
                }
            }
        }

        public float GetRatio()
        {
            int collected = 0;
            int required = 0;
            for (int i = 0; i < currentAmounts.Length; i++)
            {
                collected += currentAmounts[i];
                required += requiredAmounts[i];
            }
            if (required == 0) return 0.0f;
            return (float)collected / required;
        }
    }
    
    public bool NeedThisItem(FluidDef fluidDefinition)
    {
        for (int i = 0; i < activeTaskList.Count; i++)
        {
            TaskDef task = activeTaskList[i];
            if (task.Category != TaskCategory.SubmitItem)
            {
                continue;
            }

            SubmitItemTaskDef submitTask = (SubmitItemTaskDef)task;
            for (int r = 0; r < submitTask.requirements.Length; r++)
            {
                if (submitTask.requirements[r].fluid == fluidDefinition)
                {
                    SubmitProgressData dataValue = submitMap[submitTask.taskId];
                    int have = dataValue.currentAmounts[r];
                    int need = submitTask.requirements[r].count;
                    if (have < need) return true;
                }
            }
        }
        return false;
    }
    
    public void ForceCompleteAllTasks()
    {
        var tasks = activeTaskList.ToArray();
        foreach (var task in tasks)
        {
            MarkTaskFinished(task);
        }
    }
}