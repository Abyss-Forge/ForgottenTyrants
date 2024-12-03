using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public class Tester : MonoBehaviour
{
    [RequiredField, SerializeField] Texture2D _bomba;
    [RequiredField, SerializeField] GameObject _bomb;
    [RequiredField, SerializeField] Transform _spawnPoint;
    [RequiredField, SerializeField] float _force = 10f;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Mouse0))
        {
            GameObject instance = Instantiate(_bomb, _spawnPoint.position, quaternion.identity);

            // Calcular la dirección ajustada teniendo en cuenta el desplazamiento del spawn point respecto a la camara
            Transform camera = Camera.main.transform;
            Vector3 targetPoint = camera.position + camera.forward * 100f; // Obtener un punto lejano en la dirección de la cámara
            Vector3 adjustedDirection = (targetPoint - _spawnPoint.position).normalized;

            Rigidbody rb = instance.GetComponent<Rigidbody>();
            rb.AddForce(adjustedDirection * _force * rb.mass, ForceMode.Impulse);
        }
    }
}