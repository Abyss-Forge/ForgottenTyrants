using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AbilitiesController : MonoBehaviour
{
    [SerializeField] private AbilityStateMachine[] _abilities;

    void Start()
    {
        for (int i = 0; i < _abilities.Length;)
        {
            if (_abilities[i]._fsm != null)
            {
                _abilities[i]._fsm.SubscribeOnStateChange(LockAllOtherAbilities);
                _abilities[i]._fsm.SubscribeOnStateChange(UnlockIfNoneActive);
                i++;
            }
        }
    }

    private void LockAllOtherAbilities(EAbilityState oldState, EAbilityState newState)
    {
        if (newState == EAbilityState.ACTIVE)
        {
            foreach (AbilityStateMachine ability in _abilities)
            {
                if (ability._fsm.CurrentState.ID != EAbilityState.ACTIVE)
                {
                    ability.Lock();
                }
            }
        }
    }

    private void UnlockIfNoneActive(EAbilityState oldState, EAbilityState newState)
    {
        if (newState == EAbilityState.COOLDOWN)
        {
            for (int i = 0; i < _abilities.Length; i++)
            {
                if (_abilities[i]._fsm.CurrentState.ID == EAbilityState.LOCKED)
                {
                    _abilities[i].Unlock();
                }
            }
        }
    }

}