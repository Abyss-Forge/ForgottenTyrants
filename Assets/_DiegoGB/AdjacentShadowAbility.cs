using System.Collections;
using System.Collections.Generic;
using ForgottenTyrants;
using Systems.GameManagers;
using UnityEngine;
using UnityEngine.InputSystem;

public class AdjacentShadowAbility : MonoBehaviour
{
    [Header("Ability Settings")]
    //[SerializeField] private float _range = 10;
    [SerializeField] private float _cooldownDuration = 5, _animationDuration = 2;

    [Header("Effect Modifiers")]
    //[SerializeField] private float percentageMovementReduction = 25f;
    [SerializeField] private float _damageDealt = 30f;
    [SerializeField] private float _distanceBehind = 10f;
    [SerializeField] private float _range = 20f;
    float _cooldownTimer = 0f;
    bool _isAbilityActive = false;
    GameObject enemy;

    void Start()
    {
        MyInputManager.Instance.Subscribe(EInputAction.CLASS_ABILITY_2, OnCast, true);
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
            StartCoroutine(CastAdjacentShadow());
            _cooldownTimer = _cooldownDuration;
        }
    }

    private IEnumerator CastAdjacentShadow()
    {
        enemy = CrosshairRaycaster.GetImpactObject();
        if (enemy.CompareTag(Tag.Enemy) && CalculateIsInRange())
        {
            _isAbilityActive = true;
            TeleportToEnemy();
            yield return new WaitForSeconds(_animationDuration);
            DealDamage();
            enemy = null;
            _isAbilityActive = false;
        }
        else Debug.LogWarning("Enemy not detected or too far distance");

    }

    private bool CalculateIsInRange()
    {
        float distanceToEnemy = Vector3.Distance(transform.position, enemy.transform.position);
        return _range >= distanceToEnemy;
    }

    private void TeleportToEnemy()
    {
        Vector3 positionBehind = enemy.transform.position - enemy.transform.forward * _distanceBehind;
        GetComponent<CharacterController>().enabled = false;
        transform.position = positionBehind;
        GetComponent<CharacterController>().enabled = true;
        GetComponent<PlayerController>().SetVelocity(Vector3.zero); //Optional right now
        transform.rotation = enemy.transform.rotation;
    }

    private void DealDamage()
    {
        Debug.Log("Damage dealt: " + _damageDealt);
    }

    private void UpdateCooldownTimer()
    {
        if (_cooldownTimer > 0 && !_isAbilityActive) _cooldownTimer -= Time.deltaTime;
    }

    void OnDrawGizmos()
    {
        Gizmos.color = new(2, 1, 1, 0.3f);
        Gizmos.DrawSphere(transform.position, _range);
    }
}
