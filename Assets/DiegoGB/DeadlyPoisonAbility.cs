using System.Collections;
using System.Collections.Generic;
using System.Threading;
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
    [SerializeField] private float _timeBetweenDots = .5f;
    float _cooldownTimer = 0f;
    bool _isAbilityActive = false;
    bool _isPoisonApplied = false;
    float _poisonTimer;

    void Start()
    {
        //MyInputManager.Instance.SubscribeToInput(EInputAction.CLASS_ABILITY_1, OnCast, true);
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

    IEnumerator ApplyPoison()
    {
        _poisonTimer = 0;
        _isPoisonApplied = true;
        while (_poisonTimer < _poisonEffectTime)
        {
            Debug.Log("Dot poison " + _poisonDotDamage);
            yield return new WaitForSeconds(_timeBetweenDots);
            _poisonTimer += _timeBetweenDots;
        }
        _isPoisonApplied = false;
    }

    private void CheckPoisonApplied()
    {
        if (_isPoisonApplied)
        {
            _poisonTimer = 0;
        }
        else StartCoroutine(ApplyPoison());
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
