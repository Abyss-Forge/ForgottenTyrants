using System.Collections;
using System.Collections.Generic;
using System.Threading;
using Systems.GameManagers;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.InputSystem;

public class FireballAbility : MonoBehaviour
{
    [Header("Ability Settings")]
    [SerializeField] private float _cooldownDuration = 5, _effectDuration = 5, _animationDuration = 2;
    [SerializeField] private GameObject _fireballPrefab;

    [Header("Effect Modifiers")]
    //[SerializeField] private float percentageMovementReduction = 25f;
    [SerializeField] private float _damage = 30f;
    [SerializeField] private float _speed = 1000f;
    [SerializeField] private float _radius = 5f;
    [SerializeField] private float _lifetime = 30f;

    private float _cooldownTimer = 0f;
    private bool _isAbilityActive = false;
    private float timer;

    // Start is called before the first frame update
    void Start()
    {
        MyInputManager.Instance.Subscribe(EInputAction.CLASS_ABILITY_1, OnCast, true);
        timer = _lifetime;
    }

    // Update is called once per frame
    void Update()
    {
        UpdateCooldownTimer();
    }

    private void Cast()
    {
        if (_cooldownTimer <= 0f && !_isAbilityActive)
        {
            CastFireball();
            _cooldownTimer = _cooldownDuration;
        }
    }

    private void OnCast(InputAction.CallbackContext context)
    {
        if (context.performed) Cast();
    }

    void CastFireball()
    {
        GameObject fireball = Instantiate(_fireballPrefab, this.transform.position, quaternion.identity);

        fireball.transform.localScale = Vector3.Scale(transform.localScale, new Vector3(_radius, _radius, _radius));
        // Agregar fuerza para dispararla hacia adelante.
        Rigidbody rb = fireball.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.AddForce(transform.forward * _speed);
        }
        StartCoroutine(DeleteFireball(fireball));
        //transform.Translate(Vector3.forward * _speed * Time.deltaTime);
    }

    IEnumerator DeleteFireball(GameObject fireball)
    {
        yield return new WaitForSeconds(_lifetime);
        Destroy(fireball);

    }

    private void UpdateCooldownTimer()
    {
        if (_cooldownTimer > 0 && !_isAbilityActive) _cooldownTimer -= Time.deltaTime;
    }
}
