using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MinimapFollow : MonoBehaviour
{
    [SerializeField] private Transform _objectToFollow;
    [SerializeField] private Camera _minimapCamera;
    [SerializeField] private Vector3 _defaultPositionRespectTarget = new(0, 50, 0);
    [SerializeField] private bool _rotationEnabled, _hideShadows;

    [Header("Velocity zoom out effect")]
    [SerializeField] private float _zoomOutLerpSpeed = 1f;
    [SerializeField] private float _maxSpeedForZoomOut = 30f, _minimapZoomOutSize = 20f, _defaultMinimapSize = 10f;

    private float _storedShadowDistance;
    private float _currentSpeed;
    private Vector3 _previousPosition;

    void Awake()
    {
        _minimapCamera.gameObject.transform.position = _defaultPositionRespectTarget;
    }

    void OnEnable()
    {
        Camera.onPreCull += HandleOnPreCull;
        Camera.onPostRender += HandleOnPostRender;

        _previousPosition = _objectToFollow.position;
    }

    void OnDisable()
    {
        Camera.onPreCull -= HandleOnPreCull;
        Camera.onPostRender -= HandleOnPostRender;
    }

    void LateUpdate()
    {
        FollowTarget();
        ApplySpeedEffect();
    }

    private void FollowTarget()
    {
        Vector3 newPosition = _objectToFollow.position;
        newPosition.y = _minimapCamera.transform.position.y;
        _minimapCamera.transform.position = newPosition;

        float angle = _rotationEnabled ? _objectToFollow.eulerAngles.y : 0;
        _minimapCamera.transform.rotation = Quaternion.Euler(90f, angle, 0f);
    }

    private void ApplySpeedEffect()
    {
        _currentSpeed = Vector3.Distance(_objectToFollow.position, _previousPosition) / Time.deltaTime;
        _previousPosition = _objectToFollow.position;
        float targetSize = Mathf.Lerp(_defaultMinimapSize, _minimapZoomOutSize, _currentSpeed / _maxSpeedForZoomOut);
        _minimapCamera.orthographicSize = Mathf.Lerp(_minimapCamera.orthographicSize, targetSize, Time.deltaTime * _zoomOutLerpSpeed);
    }

    private void HandleOnPreCull(Camera cam)
    {
        if (cam == _minimapCamera && _hideShadows)
        {
            _storedShadowDistance = QualitySettings.shadowDistance;
            QualitySettings.shadowDistance = 0;
        }
    }

    private void HandleOnPostRender(Camera cam)
    {
        if (cam == _minimapCamera && _hideShadows)
        {
            QualitySettings.shadowDistance = _storedShadowDistance;
        }
    }

}
