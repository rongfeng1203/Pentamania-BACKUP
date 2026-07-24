using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class TaskBoardItem : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI nameText;
    [SerializeField] private TextMeshProUGUI descriptionText;
    [SerializeField] private Slider progressBar;
    [SerializeField] private Image icon;
    
    [HideInInspector] public TaskDef RepresentingTask;
    
    public void Initialise(TaskDef task)
    {
        RepresentingTask = task;

        if (nameText != null)
        {
            nameText.text = task.displayName;
        }

        if (descriptionText != null)
        {
            descriptionText.text = task.description;
        }
        
        if (icon != null)
        {
            icon.sprite = task.icon;
        }

        SetProgress(0f);
    }

    public void SetProgress(float value)
    {
        if (progressBar != null)
        {
            float clamped = Mathf.Clamp01(value);
            progressBar.value = clamped;
        }
    }
}