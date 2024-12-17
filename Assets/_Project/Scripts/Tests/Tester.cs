using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using JetBrains.Annotations;
using UnityEditor;

public class Tester : MonoBehaviour
{
    [SerializeField, RequiredField] string _tag;
    [RequiredField, SerializeField] GameObject _bomb;
    [RequiredField, SerializeField] Transform _spawnPoint;
    [RequiredField, SerializeField] float _force = 10f;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Mouse0))
        {
            GameObject instance = Instantiate(_bomb, _spawnPoint.position, Quaternion.identity, transform);
            instance.transform.SetParent(null); // esto es para que spawnee en la misma escena 

            Transform camera = Camera.main.transform;
            Vector3 targetPoint = camera.position + camera.forward * 100f;
            Vector3 adjustedDirection = (targetPoint - _spawnPoint.position).normalized;
            adjustedDirection.y += 0.5f;
            adjustedDirection = _spawnPoint.rotation * adjustedDirection;

            Rigidbody rb = instance.GetComponent<Rigidbody>();
            rb.AddForce(adjustedDirection * _force * rb.mass, ForceMode.Impulse);
        }
    }
}