using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class DeadlyPoisonAbility : MonoBehaviour
{
    [Header("Ability Settings")]
    //[SerializeField] private float _range = 10;
    [SerializeField] private float _effectDuration = 5, _cooldownDuration = 5, _animationDuration = 0;

    [Header("Effect Modifiers")]
    //[SerializeField] private float percentageMovementReduction = 25f;
    [SerializeField] private float _damageBoosted = 20f;
    [SerializeField] private float _poisonDotDamage = 1f;
    [SerializeField] private float _poisonEffectTime = 3f;
    float _cooldownTimer = 0f;
    bool _isAbilityActive = false;
    GameObject enemy;

    void Start()
    {
        MyInputManager.Instance.SubscribeToInput(EInputAction.CLASS_ABILITY_1, OnCast, true);
    }

    void Update()
    {
        UpdateCooldownTimer();
    }

    private void OnCast(InputAction.CallbackContext context)
    {
        if (context.performed) Cast();
    }

    private void Cast()
    {
        if (_cooldownTimer <= 0f && !_isAbilityActive)
        {
            StartCoroutine(CastDeadlyPoison());
            _cooldownTimer = _cooldownDuration;
        }
    }

    private IEnumerator CastDeadlyPoison()
    {
        _isAbilityActive = true;
        ApplyDamageBuff();
        yield return new WaitForSeconds(_effectDuration);
        ApplyDamageDebuff();
        _isAbilityActive = false;
    }

    private void ApplyPoison()
    {
        Debug.Log("Dot poison " + _poisonDotDamage);

    }

    private void ApplyDamageBuff()
    {
        Debug.Log("Buffing damage for: " + _damageBoosted);
    }

    private void ApplyDamageDebuff()
    {
        Debug.Log("Debuffing damage for: " + _damageBoosted);
    }

    private void UpdateCooldownTimer()
    {
        if (_cooldownTimer > 0 && !_isAbilityActive) _cooldownTimer -= Time.deltaTime;
    }
}
