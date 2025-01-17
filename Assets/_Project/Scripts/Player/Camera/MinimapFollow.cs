using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MinimapFollow : MonoBehaviour
{
    [SerializeField] private Transform _objectToFollow;
    [SerializeField] private Camera _minimapCamera, _minimapIconsCamera;
    [SerializeField] private Vector3 _defaultPositionRespectTarget = new(0, 50, 0);
    [SerializeField] private bool _rotationEnabled;

    [Header("Velocity zoom out effect")]
    [SerializeField] private float _zoomOutLerpSpeed = 1f;
    [SerializeField] private float _maxSpeedForZoomOut = 30f, _minimapZoomOutSize = 20f, _defaultMinimapSize = 10f;

    private float _currentSpeed;
    private Vector3 _previousPosition;

    void Awake()
    {
        _minimapCamera.transform.position = _defaultPositionRespectTarget;
        _minimapIconsCamera.transform.position = _defaultPositionRespectTarget;
    }

    void OnEnable()
    {
        _previousPosition = _objectToFollow.position;
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
        _minimapIconsCamera.transform.position = newPosition;

        if (_rotationEnabled)
        {
            Quaternion newRotation = Quaternion.Euler(90f, _objectToFollow.eulerAngles.y, 0f);
            _minimapCamera.transform.rotation = newRotation;
            _minimapIconsCamera.transform.rotation = newRotation;
        }
    }

    private void ApplySpeedEffect()
    {
        _currentSpeed = Vector3.Distance(_objectToFollow.position, _previousPosition) / Time.deltaTime;
        _previousPosition = _objectToFollow.position;
        float targetSize = Mathf.Lerp(_defaultMinimapSize, _minimapZoomOutSize, _currentSpeed / _maxSpeedForZoomOut);
        _minimapCamera.orthographicSize = Mathf.Lerp(_minimapCamera.orthographicSize, targetSize, Time.deltaTime * _zoomOutLerpSpeed);
        //TODO igualar relacion de iconos con minimapa _minimapIconsCamera.orthographicSize = _minimapCamera.orthographicSize;
    }

}
