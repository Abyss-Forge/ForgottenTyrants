using TMPro;
using UnityEngine;

public class PerformanceStats : MonoBehaviour
{
    [Header("FPS display")]
    [SerializeField] private TextMeshProUGUI _fpsText;
    [SerializeField] private float _updateRate = 1f;

    float _nextUpdateTime;
    int _frameCount, _fps;

    void Update()
    {
        _frameCount++;

        if (Time.unscaledTime >= _nextUpdateTime)
        {
            _nextUpdateTime = Time.unscaledTime + _updateRate;

            ShowFPS();
        }
    }

    private void ShowFPS()
    {
        _fps = Mathf.RoundToInt(_frameCount / _updateRate);
        _fpsText.text = $"{_fps} FPS";
        _frameCount = 0;
    }

}