using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class ShadowStepAbility : MonoBehaviour
{
    [Header("Ability Settings")]
    //[SerializeField] private float _range = 10;
    [SerializeField] private float _effectDuration = .5f, _cooldownDuration = 5, _animationDuration = 0.5f;

    [Header("Effect Modifiers")]
    //[SerializeField] private float percentageMovementReduction = 25f;
    [SerializeField] private float _charges = 3f;
    [SerializeField] private float _chargeReloadTime = 5f;

    float _cooldownTimer = 0f;
    float _reloadTimer = 0;
    bool _isAbilityActive = false;
    PlayerController _playerController;

    void Start()
    {
        MyInputManager.Instance.SubscribeToInput(EInputAction.CLASS_ABILITY_3, OnCast, true);
        _playerController = GetComponent<PlayerController>();
    }

    // Update is called once per frame
    void Update()
    {
        ReloadCharge();
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
            CastShadowStep();
            _cooldownTimer = _cooldownDuration;
        }
    }

    void CastShadowStep()
    {
        if (_charges > 0)
        {
            StartCoroutine(_playerController.Dash());
            Debug.Log("nigs");
            _charges--;
        }
    }

    void ReloadCharge()
    {
        if (!HasMaxCharges())
        {
            _reloadTimer += Time.deltaTime;
            if (_reloadTimer >= _chargeReloadTime)
            {
                _reloadTimer = 0;
                _charges++;
            }
        }
    }

    void EnemyKilled()
    {
        Debug.Log("Enemy killed");
        if (!HasMaxCharges())
        {
            _charges++;
        }
    }

    bool HasMaxCharges()
    {
        return (_charges == 3) ? true : false;
    }

    private void UpdateCooldownTimer()
    {
        if (_cooldownTimer > 0 && !_isAbilityActive) _cooldownTimer -= Time.deltaTime;
    }
}
