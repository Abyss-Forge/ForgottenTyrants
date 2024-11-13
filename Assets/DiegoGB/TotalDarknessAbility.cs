using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;
using UnityEngine.UI;

public class TotalDarknessAbility : MonoBehaviour
{
    [Header("Ability Settings")]
    //[SerializeField] private float _range = 10;
    [SerializeField] private float _cooldownDuration = 5, _effectDuration = 5, _animationDuration = 2;
    [SerializeField] private Material _newMaterial;

    [Header("Effect Modifiers")]
    //[SerializeField] private float percentageMovementReduction = 25f;
    [SerializeField] private float _percentageMovementBoost = 25f;


    [Header("UI Settings")]
    [SerializeField] private TextMeshProUGUI _cooldownText;
    [SerializeField] private Image _cooldownImage;

    private float _cooldownTimer = 0f;
    private bool _isAbilityActive = false;

    private void OnCast(InputAction.CallbackContext context)
    {
        if (context.performed) Cast();
    }

    void Start()
    {
        MyInputManager.Instance.SubscribeToInput(EInputActions.ClassAbility2, OnCast, true);
    }

    void Update()
    {
        UpdateCooldownTimer();
        UpdateCooldownText();
        UpdateCooldownImage();
    }

    private void Cast()
    {
        if (_cooldownTimer <= 0f && !_isAbilityActive)
        {
            StartCoroutine(ApplyGhostEffect());
            _cooldownTimer = _cooldownDuration;
        }
    }

    private void UpdateCooldownText()
    {
        if (_cooldownText != null)
        {
            if (_cooldownTimer > 0 && !_isAbilityActive)
                _cooldownText.text = $"{_cooldownTimer:F1}";
            else
                _cooldownText.text = "";
        }
    }

    private void UpdateCooldownImage()
    {
        if (_cooldownImage != null)
        {
            if (_cooldownTimer > 0)
            {
                _cooldownImage.fillAmount = _cooldownTimer / _cooldownDuration;
            }
            else
            {
                _cooldownImage.fillAmount = 0;
            }
        }
    }

    private IEnumerator ApplyGhostEffect()
    {
        _isAbilityActive = true;
        Debug.Log("casting");
        ChangePlayerColor();
        GhostStatusEffect ghostStatusEffect = new GhostStatusEffect();
        ghostStatusEffect.ApplyEffect(this.gameObject.GetComponent<Player>());
        yield return new WaitForSeconds(_effectDuration);
        ghostStatusEffect.RemoveEffect(this.gameObject.GetComponent<Player>());
        _isAbilityActive = false;
        Debug.Log("end casting");
    }

    private void UpdateCooldownTimer()
    {
        if (_cooldownTimer > 0 && !_isAbilityActive) _cooldownTimer -= Time.deltaTime;
    }

    private void ChangePlayerColor()
    {
        MeshRenderer meshRenderer = GetComponent<MeshRenderer>();
        // Obtén el array de materiales
        Material[] materials = meshRenderer.materials;

        // Cambia un material específico
        materials[0] = _newMaterial; // Cambia el primer material

        // Asigna el array de vuelta al MeshRenderer
        meshRenderer.materials = materials;
    }
}
