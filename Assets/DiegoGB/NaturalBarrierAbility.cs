using System.Collections;
using System.Collections.Generic;
using ForgottenTyrants;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.InputSystem;

public class NaturalBarrierAbility : MonoBehaviour
{
    [Header("Ability Settings")]
    [SerializeField] private GameObject _barrierPrefab;
    [SerializeField] private float _cooldownDuration = 5, _animationDuration = 2;

    [Header("Effect Modifiers")]
    //[SerializeField] private float percentageMovementReduction = 25f;
    [SerializeField] private float _duration = 30f;
    [SerializeField] private float _range = 20f;
    float _cooldownTimer = 0f;
    bool _isAbilityActive = false;
    Vector3? _spawnPosition = null;
    GameObject _impactObject = null;


    void Start()
    {
        MyInputManager.Instance.SubscribeToInput(EInputAction.CLASS_ABILITY_4, OnCast, true);
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
        _impactObject = MyCursorManager.Instance.GetCrosshairImpactObject();
        _spawnPosition = MyCursorManager.Instance.GetCrosshairImpactPoint();

        if (_impactObject == null || _spawnPosition == null) return;

        if (_cooldownTimer <= 0f && !_isAbilityActive && CheckIfIsCastable() && CheckInRange())
        {
            StartCoroutine(CastNaturalBarrier());
            _cooldownTimer = _cooldownDuration;
        }
    }

    bool CheckIfIsCastable()
    {
        bool isCastable = true;
        if (_impactObject.layer != Layer.Terrain)
        {
            Debug.Log("No es terreno!");
            isCastable = false;
        }
        return isCastable;
    }

    bool CheckInRange()
    {
        float distance = Vector3.Distance(transform.position, _spawnPosition.Value);
        if (distance > _range)
        {
            Debug.Log($"¡Fuera de rango! Distancia: {distance}, Rango permitido: {_range}");
            return false;
        }
        return true;
    }

    IEnumerator CastNaturalBarrier()
    {
        _isAbilityActive = true;
        GameObject barrier = Instantiate(_barrierPrefab, AdjustBarrierToTerrain(), Quaternion.LookRotation(-transform.right, Vector3.up));
        yield return new WaitForSeconds(_duration);
        Destroy(barrier);
        _isAbilityActive = false;
    }

    private Vector3 AdjustBarrierToTerrain()
    {
        Vector3 spawnPositionAdjusted = _spawnPosition.Value;

        float halfHeight = 0f;
        Renderer renderer = _barrierPrefab.GetComponent<Renderer>();
        halfHeight = renderer.bounds.size.y / 2;

        spawnPositionAdjusted.y += halfHeight;

        return spawnPositionAdjusted;
    }

    private void UpdateCooldownTimer()
    {
        if (_cooldownTimer > 0 && !_isAbilityActive) _cooldownTimer -= Time.deltaTime;
    }
}
