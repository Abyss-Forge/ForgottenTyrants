using TMPro;
using UnityEngine;

public class PerformanceStatsScript : MonoBehaviour
{
    [Header("FPS display")]

    [SerializeField] private TextMeshProUGUI FpsText;
    [SerializeField] private float UpdateRate = 1f;

    private float lastUpdate = 0f;
    private float frames = 0f;
    private float fps = 0f;

    void Update()
    {
        frames++;
        float currentTime = Time.realtimeSinceStartup;
        if (currentTime - lastUpdate >= UpdateRate)
        {
            fps = frames / (currentTime - lastUpdate);
            FpsText.text = Mathf.RoundToInt(fps) + " FPS";
            frames = 0;
            lastUpdate = currentTime;
        }
    }

}
