using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MinimapFollow : MonoBehaviour
{
    [SerializeField] private Transform _objectToFollow;
    [SerializeField] private Camera _minimapCamera;
    [SerializeField] private bool _rotationEnabled, _hideShadows;
    private float _storedShadowDistance;

    void LateUpdate()
    {
        Vector3 newPosition = _objectToFollow.position;
        newPosition.y = _minimapCamera.transform.position.y;
        _minimapCamera.transform.position = newPosition;

        float angle = _rotationEnabled ? _objectToFollow.eulerAngles.y : 0;
        _minimapCamera.transform.rotation = Quaternion.Euler(90f, angle, 0f);
    }

    void OnEnable()
    {
        Camera.onPreCull += HandleOnPreCull;
        Camera.onPostRender += HandleOnPostRender;
    }

    void OnDisable()
    {
        Camera.onPreCull -= HandleOnPreCull;
        Camera.onPostRender -= HandleOnPostRender;
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
