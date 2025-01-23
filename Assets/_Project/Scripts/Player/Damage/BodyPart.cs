using System;
using UnityEngine;

[RequireComponent(typeof(Rigidbody), typeof(Collider))]
public class BodyPart : MonoBehaviour
{
    public Rigidbody Rigidbody { get; private set; }
    public Collider Collider { get; private set; }

    public Action<Collision> OnCollision;
    public Action<Collider> OnTrigger;

    void Awake()
    {
        Rigidbody = GetComponent<Rigidbody>();
        Collider = GetComponent<Collider>();
    }

    void OnCollisionEnter(Collision other)
    {
        OnCollision?.Invoke(other);
    }

    void OnTriggerEnter(Collider other)
    {
        OnTrigger?.Invoke(other);
    }

}