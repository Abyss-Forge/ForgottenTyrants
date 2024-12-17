using System;
using UnityEngine;

[RequireComponent(typeof(Rigidbody), typeof(Collider))]
public class BodyPart : MonoBehaviour
{
    public Rigidbody Rigidbody { get; private set; }
    public Collider Collider { get; private set; }

    public Action<Collision> OnCollision;

    void Awake()
    {
        Rigidbody = GetComponent<Rigidbody>();
        Collider = GetComponent<Collider>();
    }

    void OnCollisionEnter(Collision other)
    {
        OnCollision?.Invoke(other);
    }

}