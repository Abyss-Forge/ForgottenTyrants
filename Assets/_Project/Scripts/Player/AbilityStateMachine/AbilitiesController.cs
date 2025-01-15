using System.Collections;
using System.Collections.Generic;
using Systems.GameManagers;
using UnityEngine;
using UnityEngine.InputSystem;
using Utils.Extensions;

public class AbilitiesController : MonoBehaviour
{
    [SerializeField] RectTransform _abilitiesIconsHolder;
    [SerializeField] Transform _abilitiesHolder;

    ClientData _playerData;
    List<AbilityStateMachine> _abilities;

    void Awake()
    {
        _playerData = HostManager.Instance.GetMyClientData();
        _abilities = new();

        foreach (AbilityTemplate template in _playerData.Class.Abilities)
        {
            var ability = ExtensionMethods.GetInstantiate<AbilityStateMachine>(template.AbilityPrefab.gameObject, _abilitiesHolder);
            _abilities.Add(ability);

            var icon = ExtensionMethods.GetInstantiate<AbilityIcon>(template.IconPrefab.gameObject, _abilitiesIconsHolder);
            template.InitializeSprites();
            icon.Initialize(ability);
        }
    }

    void OnEnable()
    {
        MyInputManager.Instance.Subscribe(EInputAction.CLASS_ABILITY_1, OnClassAbility1);
        MyInputManager.Instance.Subscribe(EInputAction.CLASS_ABILITY_2, OnClassAbility2);
        MyInputManager.Instance.Subscribe(EInputAction.CLASS_ABILITY_3, OnClassAbility3);
        MyInputManager.Instance.Subscribe(EInputAction.CLASS_ABILITY_4, OnClassAbility4);
    }

    void OnDisable()
    {
        MyInputManager.Instance.Unsubscribe(EInputAction.CLASS_ABILITY_1, OnClassAbility1);
        MyInputManager.Instance.Unsubscribe(EInputAction.CLASS_ABILITY_2, OnClassAbility2);
        MyInputManager.Instance.Unsubscribe(EInputAction.CLASS_ABILITY_3, OnClassAbility3);
        MyInputManager.Instance.Unsubscribe(EInputAction.CLASS_ABILITY_4, OnClassAbility4);
    }

    private void OnClassAbility1(InputAction.CallbackContext context) { if (context.performed) _abilities[0].Trigger(); }
    private void OnClassAbility2(InputAction.CallbackContext context) { if (context.performed) _abilities[1].Trigger(); }
    private void OnClassAbility3(InputAction.CallbackContext context) { if (context.performed) _abilities[2].Trigger(); }
    private void OnClassAbility4(InputAction.CallbackContext context) { if (context.performed) _abilities[3].Trigger(); }

    void Start()
    {
        for (int i = 0; i < _abilities.Count;)
        {
            if (_abilities[i]._fsm != null)
            {
                _abilities[i]._fsm.SubscribeOnStateChange(HandleAbilityStates);
                i++;    //TODO: remake using event bus
                        //esto es inseguro de narices pero esta hecho asi pq a veces las habilidades tardan en instanciarse y
                        //necesitamos que se suscriban a los eventos cuando hayan ternimando de inicializarse,
            }
        }
    }

    private void HandleAbilityStates(EAbilityState oldState, EAbilityState newState)
    {
        foreach (AbilityStateMachine ability in _abilities)
        {
            if (newState == EAbilityState.ACTIVE && ability._fsm.CurrentState.ID != EAbilityState.ACTIVE)   //Lock every ability but the active one
            {
                ability.Lock();
            }
            else if (newState == EAbilityState.COOLDOWN && ability._fsm.CurrentState.ID == EAbilityState.LOCKED)    //Unlock every ability when the active one goes on cooldown
            {
                ability.Unlock();
            }
        }
    }

}