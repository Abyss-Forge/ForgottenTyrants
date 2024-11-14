using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AbilityCooldownState : State<EAbilityState>
{
    AbilityStateMachine _ability;
    public AbilityCooldownState(AbilityStateMachine ability) : base(EAbilityState.COOLDOWN)
    {
        _ability = ability;
    }

    public override void Update()
    {
        base.Update();

        UpdateCooldownTimer();
    }

    public override void Enter()
    {
        base.Enter();

        _ability.CooldownText.gameObject.SetActive(true);
    }

    public override void Exit()
    {
        base.Exit();

        _ability.CooldownTimer = _ability.CooldownDuration;

        _ability.CooldownImage.fillAmount = 0;
        _ability.CooldownText.text = "";
        _ability.CooldownImage.gameObject.SetActive(false);
    }

    private void UpdateCooldownTimer()
    {
        if (_ability.CooldownTimer > 0)
        {
            _ability.CooldownTimer -= Time.deltaTime;

            _ability.CooldownImage.fillAmount = _ability.CooldownTimer / _ability.CooldownDuration;
            _ability.CooldownText.text = $"{_ability.CooldownTimer:F1}";
        }
        else
        {
            _ability._fsm.SetCurrentState(EAbilityState.READY);
        }
    }
}