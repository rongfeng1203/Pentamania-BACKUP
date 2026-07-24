using UnityEngine;

public class TaskDebugPanel : MonoBehaviour
{
    private void OnGUI()
    {
        if (GUILayout.Button("Clear Progress"))
            TaskManager.Instance.ResetAllProgress();

        if (GUILayout.Button("Finish the current task"))
        {
            var list = TaskManager.Instance.ActiveTasks;
            //if (list.Count > 0)
                //TaskManager.Instance.AddProgress(list[0], list[0].);
        }
    }
}