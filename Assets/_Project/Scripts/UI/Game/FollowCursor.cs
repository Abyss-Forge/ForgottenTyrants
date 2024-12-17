using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowCursor : MonoBehaviour
{
    [SerializeField] private float _offset = 1f;
    private Vector3 _position;

    void Update()
    {
        _position = Input.mousePosition;
        _position.z = _offset;
        transform.position = Camera.main.ScreenToWorldPoint(_position);
    }
}

