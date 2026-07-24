using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

[DefaultExecutionOrder(100)]
public class TaskAssigner : MonoBehaviour
{
    public static TaskAssigner Instance { get; private set; }
    
    [SerializeField] private TaskDef[] taskSequence;

    private int nextIndex;

    public string endMenu;

    public List<Animator> tentacles;
    
    public float moveDistance = 1f;
    public float moveDuration = 1f;
    public float delayBetweenStarts = 0.2f;

    void Awake()
    {
        if (Instance != null) { Destroy(gameObject); return; }
        Instance = this;

        nextIndex = 0;
        TryAssignNext();
        
    }
    
    public void TryAssignNext()
    {
        if (TaskManager.Instance.ActiveTasks.Count > 0)
        {
            Debug.Log("[Task System] One or more task is running");
            return;
        }

        if (nextIndex >= taskSequence.Length)
        {
            Debug.Log("You finished the game, thank you for playing our game -- Us");
            SpawnTentacles();
            StartCoroutine(DelayedEndGame());
            StartMovingAll();
            return;
        }

        TaskManager.Instance.PushTask(taskSequence[nextIndex]);
        nextIndex++;
    }

    private void SpawnTentacles()
    {
        foreach (var tentacle in tentacles)
        {
            tentacle.enabled = true;
        }
    }

    private IEnumerator DelayedEndGame()
    {
        Debug.Log("[Task System] Waiting 30s before ending the game");
        yield return new WaitForSeconds(30f);

        SceneManager.LoadScene(endMenu);
    }
    
    public void StartMovingAll()
    {
        StartCoroutine(StaggeredStart());
    }

    private IEnumerator StaggeredStart()
    {
        foreach (var obj in tentacles)
        {
            StartCoroutine(MoveOne(obj.gameObject.transform));
            yield return new WaitForSeconds(delayBetweenStarts);
        }
    }

    private IEnumerator MoveOne(Transform obj)
    {
        Vector3 startPos = obj.localPosition;
        Vector3 endPos = startPos + obj.TransformDirection(Vector3.down) * moveDistance;

        float elapsed = 0f;
        while (elapsed < moveDuration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / moveDuration);
            obj.localPosition = Vector3.Lerp(startPos, endPos, t);
            yield return null;
        }

        obj.localPosition = endPos;
    }
}