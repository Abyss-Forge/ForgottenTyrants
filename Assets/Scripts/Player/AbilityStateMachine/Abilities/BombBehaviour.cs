using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ForgottenTyrants;

public class BombBehaviour : ExplosiveProjectile
{
    void OnDrawGizmos()
    {
        Gizmos.color = new(0, 1, 0, 0.3f);
        Gizmos.DrawSphere(transform.position, _explosionRadius);
    }

    protected override void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.CompareTag(Tag.Enemy))
        {
            _isDirectHit = true;
        }
        else if (_canBounce && _remainingBounces > 0)
        {
            // Calcular el ángulo entre la normal de la superficie y la dirección de movimiento
            Vector3 collisionNormal = other.contacts[0].normal;
            Vector3 incomingDirection = _rigidbody.velocity.normalized;
            float angle = Vector3.Angle(incomingDirection, -collisionNormal) - 90;

            if (angle <= _maxBounceAngle)
            {
                _remainingBounces--;
                return;
                //  Vector3 reflectedDirection = Vector3.Reflect(incomingDirection, collisionNormal);
                //  _rigidbody.velocity = reflectedDirection * _rigidbody.velocity.magnitude;
            }
        }

        OnHit();
    }

    protected override void OnHit()
    {
        Explode(_isDirectHit);
        //vfx & sound
        Destroy(gameObject);
    }

    private void Explode(bool isDirectHit)
    {
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, _explosionRadius);

        foreach (Collider hitCollider in hitColliders)
        {
            if (hitCollider.gameObject.CompareTag(Tag.Enemy))
            {
                float damage, knockback;
                if (isDirectHit)
                {
                    damage = _damage;
                    knockback = _knockbackForce;
                }
                else
                {
                    float distance = Vector3.Distance(hitCollider.transform.position, transform.position);
                    float normalizedDistance = 1 - Mathf.Clamp01(distance / _explosionRadius);
                    float effectPercentage = _explosionAreaCurve.Evaluate(normalizedDistance);
                    damage = _damage * effectPercentage;
                    knockback = _knockbackForce * effectPercentage;
                }

                hitCollider.gameObject.GetComponentInChildren<Rigidbody>()?.AddExplosionForce(knockback, transform.position, _explosionRadius, 3f);

                Debug.Log($"Enemy takes {damage} damage");
            }

        }
    }


}