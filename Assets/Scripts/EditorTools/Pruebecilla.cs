using UnityEngine;
using UnityEditor;

[ExecuteInEditMode]
public class AspectScale : MonoBehaviour
{
    private Vector3 _baseScale;
    private Vector3 _current;
    private Transform _transform;

    private void OnEnable()
    {
        _transform = transform;
        _baseScale = _transform.localScale;
        _current = _baseScale;
    }

    private void Update()
    {
        if (_transform.hasChanged)
        {
            if (_current.x != _transform.localScale.x)
                SetScale(_transform.localScale.x / _baseScale.x);
            else if (_current.y != _transform.localScale.y)
                SetScale(_transform.localScale.y / _baseScale.y);
            else if (_current.z != _transform.localScale.z)
                SetScale(_transform.localScale.z / _baseScale.z);
        }
    }

    private void SetScale(float value)
    {
        _transform.localScale = _baseScale * value;
        _current = _transform.localScale;
    }
}