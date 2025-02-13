using System;
using UnityEngine;

[RequireComponent(typeof(Rigidbody), typeof(Collider))]
public class BodyPart : MonoBehaviour
{
    public Rigidbody Rigidbody { get; private set; }
    public Collider Collider { get; private set; }

    public event Action<Collision> OnCollisionEnterEvent, OnCollisionStayEvent, OnCollisionExitEvent;
    public event Action<Collider> OnTriggerEnterEvent, OnTriggerStayEvent, OnTriggerExitEvent;

    void Awake()
    {
        Rigidbody = GetComponent<Rigidbody>();
        Collider = GetComponent<Collider>();
    }

    void OnCollisionEnter(Collision other) => OnCollisionEnterEvent?.Invoke(other);
    void OnCollisionStay(Collision other) => OnCollisionStayEvent?.Invoke(other);
    void OnCollisionExit(Collision other) => OnCollisionExitEvent?.Invoke(other);

    void OnTriggerEnter(Collider other) => OnTriggerEnterEvent?.Invoke(other);
    void OnTriggerStay(Collider other) => OnTriggerStayEvent?.Invoke(other);
    void OnTriggerExit(Collider other) => OnTriggerExitEvent?.Invoke(other);

}