using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class AbilityIcon : MonoBehaviour
{
    [SerializeField] private Image _iconImage, _backgroundImage, _borderImage;
    [SerializeField] private GameObject _activeOverlay, _cooldownOverlay, _lockedOverlay;
    [SerializeField] private Image _cooldownImage;
    [SerializeField] private TextMeshProUGUI _cooldownText;

    private EAbilityState _currentState;
    private AbilityStateMachine _abilityStateMachine;

    public void Initialize(AbilityStateMachine abilityStateMachine)
    {
        _abilityStateMachine = abilityStateMachine;
        _abilityStateMachine.FSM.SubscribeOnStateChange(OnStateChange);
    }

    public void SetSprites(Sprite iconImage, Sprite backgroundImage, Sprite borderImage)
    {
        _iconImage.sprite = iconImage;
        _backgroundImage.sprite = backgroundImage;
        _borderImage.sprite = borderImage;
    }

    void Update()
    {
        if (_abilityStateMachine != null && _currentState == EAbilityState.COOLDOWN || _currentState == EAbilityState.LOCKED)
        {
            OnUpdateCooldown(_abilityStateMachine.CooldownTimer, _abilityStateMachine.CooldownDuration);
        }
    }

    private void OnStateChange(EAbilityState oldState, EAbilityState newState)
    {
        _currentState = _abilityStateMachine.FSM.CurrentState.ID;

        if (newState == EAbilityState.READY)
        {
            _lockedOverlay.SetActive(false);
            _cooldownOverlay.SetActive(false);
        }

        if (newState == EAbilityState.ACTIVE)
        {
            _cooldownImage.fillAmount = 1;
            _activeOverlay.SetActive(true);
        }

        if (newState == EAbilityState.COOLDOWN)
        {
            _activeOverlay.SetActive(false);
            _cooldownOverlay.SetActive(true);
        }

        if (newState == EAbilityState.LOCKED)
        {
            _lockedOverlay.SetActive(true);
        }
    }

    public void OnEnterActive()
    {
        _cooldownImage.fillAmount = 1;
        _activeOverlay.SetActive(true);
    }

    public void OnEnterCooldown()
    {
        _activeOverlay.SetActive(false);
        _cooldownImage.gameObject.SetActive(true);
    }

    public void OnUpdateCooldown(float cooldownTimer, float cooldownDuration)
    {
        _cooldownImage.fillAmount = cooldownTimer / cooldownDuration;
        _cooldownText.text = $"{cooldownTimer:F1}"; //1 decimal
        if (cooldownTimer <= 0)
        {
            _cooldownText.text = null;
        }
    }

    public void OnExitCooldown()
    {
        _cooldownImage.gameObject.SetActive(false);
    }

    public void OnEnterLocked()
    {
        _lockedOverlay.SetActive(true);
    }

    public void OnExitLocked()
    {
        _lockedOverlay.SetActive(false);
    }
}