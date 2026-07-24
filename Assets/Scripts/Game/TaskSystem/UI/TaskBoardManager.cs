using System.Collections.Generic;
using UnityEngine;

[DefaultExecutionOrder(120)]
public class TaskBoardManager : MonoBehaviour
{
    [SerializeField] private TaskBoardItem boardItemPrefab;
    [SerializeField] private Transform contentRoot;

    private readonly List<TaskBoardItem> spawnedList = new List<TaskBoardItem>();
    
    private void OnEnable()
    {
        RebuildBoard(TaskManager.Instance.ActiveTasks);
        TaskManager.Instance.ProgressChanged += HandleProgressChanged;
        TaskManager.Instance.TaskCompleted += HandleTaskCompleted;
        TaskManager.Instance.TaskAssigned += HandleTaskAssigned;
    }

    private void OnDisable()
    {
        if (TaskManager.Instance != null)
        {
            TaskManager.Instance.ProgressChanged -= HandleProgressChanged;
            TaskManager.Instance.TaskCompleted -= HandleTaskCompleted;
            TaskManager.Instance.TaskAssigned -= HandleTaskAssigned;
        }
    }
    
    public void RebuildBoard(IEnumerable<TaskDef> taskSet)
    {
        ClearBoard();

        if (taskSet == null)
        {
            Debug.LogWarning("[Task Board] No tasks");
            return;
        }

        foreach (TaskDef eachTask in taskSet)
        {
            if (eachTask == null)
            {
                continue;
            }

            TaskBoardItem newItem = Instantiate(boardItemPrefab, contentRoot);
            newItem.gameObject.SetActive(true);
            newItem.Initialise(eachTask);

            spawnedList.Add(newItem);
            Debug.Log("[Task Board] Generated task bars: " + eachTask.displayName);
        }
    }
    
    private void ClearBoard()
    {
        for (int i = 0; i < spawnedList.Count; i++)
        {
            if (spawnedList[i] != null)
            {
                Destroy(spawnedList[i].gameObject);
            }
        }
        spawnedList.Clear();
        Debug.Log("[Task Board] Task board cleared");
    }
    
    private void HandleProgressChanged(TaskDef task, float ratio)
    {
        for (int i = 0; i < spawnedList.Count; i++)
        {
            if (spawnedList[i].RepresentingTask == task)
            {
                spawnedList[i].SetProgress(ratio);
                Debug.Log("[TaskBoard] Progress updated: " + task.displayName + " " + ratio);
                return;
            }
        }
    }

    private void HandleTaskCompleted(TaskDef task)
    {
        RebuildBoard(TaskManager.Instance.ActiveTasks);
        Debug.Log("[Task Board] Task board refreshed: " + task.displayName);
    }
    
    private void HandleTaskAssigned()
    {
        RebuildBoard(TaskManager.Instance.ActiveTasks);
        Debug.Log("[Task Board] Task board refreshed");
    }
}