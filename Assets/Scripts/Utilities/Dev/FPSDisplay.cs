using UnityEngine;

public class FPSDisplay : MonoBehaviour
{
    [SerializeField] private float updateInterval = 0.5f;

    private float timeLeft;
    private float fps;
    private GUIStyle style;
    private Rect rect;

    void Start()
    {
        timeLeft = updateInterval;

        style = new GUIStyle();
        style.fontSize = 24;
        style.fontStyle = FontStyle.Bold;
        style.normal.textColor = Color.white;

        rect = new Rect(10, 10, 250, 40);
    }

    void Update()
    {
        timeLeft -= Time.deltaTime;

        if (timeLeft <= 0f)
        {
            fps = 1f / Time.unscaledDeltaTime;
            timeLeft = updateInterval;
        }
    }

    void OnGUI()
    {
        if (fps > 50) style.normal.textColor = Color.green;
        else if (fps > 30) style.normal.textColor = Color.yellow;
        else style.normal.textColor = Color.red;

        GUI.Label(rect, $"FPS: {fps:F1}", style);
    }
}