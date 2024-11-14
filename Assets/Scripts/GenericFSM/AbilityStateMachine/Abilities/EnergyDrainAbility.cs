using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class EnergyDrainAbility : MonoBehaviour
{

    [Header("Ability Settings")]
    //[SerializeField] private float _range = 10;
    [SerializeField] private float _cooldownDuration = 5, _effectDuration = 5, _animationDuration = 2;
    [SerializeField] private int _dots = 5;

    [Header("Effect Modifiers")]
    //[SerializeField] private float percentageMovementReduction = 25f;
    [SerializeField] private float _damagePerDot = 3f;
    private float _cooldownTimer = 0f;
    private bool _isAbilityActive = false;



    private void OnCast(InputAction.CallbackContext context)
    {
        if (context.performed) Cast();
    }

    void Start()
    {
        MyInputManager.Instance.SubscribeToInput(EInputActions.ClassAbility3, OnCast, true);
    }

    void Update()
    {
        UpdateCooldownTimer();
    }

    private void Cast()
    {
        if (_cooldownTimer <= 0f && !_isAbilityActive)
        {
            StartCoroutine(ApplyDamageAbsorptionEffect());
            _cooldownTimer = _cooldownDuration;
        }
    }

    private IEnumerator ApplyDamageAbsorptionEffect()
    {
        _isAbilityActive = true;
        GameObject objective = MyCursorManager.Instance.GetCrosshairTarget();
        Debug.Log("casting energy drain");
        GhostStatusEffect ghostStatusEffect = new GhostStatusEffect();
        ghostStatusEffect.ApplyEffect(this.gameObject.GetComponent<Player>());
        for (int i = 0; i < _dots; i++)
        {
            Debug.Log("absorbing 5 dmg from - " + objective + "at " + System.DateTime.Now);
            yield return new WaitForSeconds(_effectDuration / _dots);
        }
        ghostStatusEffect.RemoveEffect(this.gameObject.GetComponent<Player>());
        _isAbilityActive = false;
    }

    private void UpdateCooldownTimer()
    {
        if (_cooldownTimer > 0 && !_isAbilityActive) _cooldownTimer -= Time.deltaTime;
    }

    void OnDrawGizmos()
    {
        if (_isAbilityActive)
        {
            Gizmos.color = new(0, 0, 1, 0.3f);
            Gizmos.DrawSphere(transform.position, 30);
        }
    }
}
