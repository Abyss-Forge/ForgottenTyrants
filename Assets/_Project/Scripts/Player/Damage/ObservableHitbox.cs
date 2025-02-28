using System;
using UnityEngine;
using Utils.Extensions;

[RequireComponent(typeof(Rigidbody), typeof(Collider))]
public class ObservableHitbox : MonoBehaviour
{
    protected Rigidbody _rigidbody;
    internal Rigidbody Rigidbody => _rigidbody.OrNull() ?? (_rigidbody = GetComponent<Rigidbody>());

    protected Collider _collider;
    internal Collider Collider => _collider.OrNull() ?? (_collider = GetComponent<Collider>());

    public event Action<Collision> OnCollisionEnterEvent, OnCollisionStayEvent, OnCollisionExitEvent;
    public event Action<Collider> OnTriggerEnterEvent, OnTriggerStayEvent, OnTriggerExitEvent;

    void OnCollisionEnter(Collision other) => OnCollisionEnterEvent?.Invoke(other);
    void OnCollisionStay(Collision other) => OnCollisionStayEvent?.Invoke(other);
    void OnCollisionExit(Collision other) => OnCollisionExitEvent?.Invoke(other);

    void OnTriggerEnter(Collider other) => OnTriggerEnterEvent?.Invoke(other);
    void OnTriggerStay(Collider other) => OnTriggerStayEvent?.Invoke(other);
    void OnTriggerExit(Collider other) => OnTriggerExitEvent?.Invoke(other);

    public void Initialize()
    {
        _rigidbody = null;
        _collider = null;
    }

}