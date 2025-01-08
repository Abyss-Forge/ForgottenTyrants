using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AbilitiesController : MonoBehaviour
{
    [SerializeField] RectTransform _abilitiesParent;

    ClientData _playerData;
    List<AbilityStateMachine> _abilities;

    void Awake()
    {
        _playerData = HostManager.Instance.GetMyClientData();

        foreach (AbilityTemplate ability in _playerData.Class.Abilities)
        {
            Instantiate(ability.AbilityPrefab, transform);
            _abilities.Add(ability.AbilityPrefab);
            Instantiate(ability.IconPrefab, _abilitiesParent);
            ability.InitializeSprites();
        }
    }

    void Start()
    {
        for (int i = 0; i < _abilities.Count;)
        {
            if (_abilities[i]._fsm != null)
            {
                _abilities[i]._fsm.SubscribeOnStateChange(LockAllOtherAbilities);
                _abilities[i]._fsm.SubscribeOnStateChange(UnlockIfNoneActive);
                i++;    //TODO: remake using event bus
                        //esto es inseguro de narices pero esta hecho asi pq a veces las habilidades tardan en instanciarse y
                        //necesitamos que se suscriban a los eventos cuando hayan ternimando de inicializarse,
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
            for (int i = 0; i < _abilities.Count; i++)
            {
                if (_abilities[i]._fsm.CurrentState.ID == EAbilityState.LOCKED)
                {
                    _abilities[i].Unlock();
                }
            }
        }
    }

}