using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class PerformanceStatsController : MonoBehaviour
{
    [Header("FPS display")]
    [SerializeField] private TextMeshProUGUI _fpsText;
    [SerializeField] private float _textUpdateRate = 1f;

    private float _lastUpdate, _frames, _fps;

    void Start()
    {
        _lastUpdate = 0f;
        _frames = 0f;
        _fps = 0f;
    }

    void Update()
    {
        _frames++;
        float currentTime = Time.realtimeSinceStartup;
        if (currentTime - _lastUpdate >= _textUpdateRate)
        {
            _fps = _frames / (currentTime - _lastUpdate);
            _fpsText.text = Mathf.RoundToInt(_fps) + " FPS";
            _frames = 0;
            _lastUpdate = currentTime;
        }
    }

}
