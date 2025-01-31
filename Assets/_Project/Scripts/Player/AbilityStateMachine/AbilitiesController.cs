using System.Collections;
using System.Collections.Generic;
using Systems.GameManagers;
using Systems.ServiceLocator;
using UnityEngine;
using UnityEngine.InputSystem;
using Utils.Extensions;

public class AbilitiesController : MonoBehaviour
{
    [SerializeField] RectTransform _abilitiesIconsHolder;
    [SerializeField] Transform _abilitiesHolder;

    List<AbilityStateMachine> _abilities;

    void Awake()
    {
        _abilities = new();

        ServiceLocator.Global.Get(out PlayerInfo player);

        foreach (AbilityTemplate template in player.ClientData.Class.Abilities)
        {
            var ability = ExtensionMethods.InstantiateAndGet<AbilityStateMachine>(template.AbilityPrefab.gameObject, _abilitiesHolder);
            _abilities.Add(ability);

            var icon = ExtensionMethods.InstantiateAndGet<AbilityIcon>(template.IconPrefab.gameObject, _abilitiesIconsHolder);
            template.InitializeSprites();
            icon.Initialize(ability);
        }
    }

    void OnEnable()
    {
        MyInputManager.Instance.Subscribe(EInputAction.CLASS_ABILITY_1, _abilities[0].OnTriggered);
        MyInputManager.Instance.Subscribe(EInputAction.CLASS_ABILITY_2, _abilities[1].OnTriggered);
        MyInputManager.Instance.Subscribe(EInputAction.CLASS_ABILITY_3, _abilities[2].OnTriggered);
        MyInputManager.Instance.Subscribe(EInputAction.CLASS_ABILITY_4, _abilities[3].OnTriggered);
    }

    void OnDisable()
    {
        MyInputManager.Instance.Unsubscribe(EInputAction.CLASS_ABILITY_1, _abilities[0].OnTriggered);
        MyInputManager.Instance.Unsubscribe(EInputAction.CLASS_ABILITY_2, _abilities[1].OnTriggered);
        MyInputManager.Instance.Unsubscribe(EInputAction.CLASS_ABILITY_3, _abilities[2].OnTriggered);
        MyInputManager.Instance.Unsubscribe(EInputAction.CLASS_ABILITY_4, _abilities[3].OnTriggered);
    }

    void Start()
    {
        for (int i = 0; i < _abilities.Count;)
        {
            if (_abilities[i].FSM != null)
            {
                _abilities[i].FSM.SubscribeOnStateChange(HandleAbilityStates);
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
            if (newState == EAbilityState.ACTIVE && ability.FSM.CurrentState.ID != EAbilityState.ACTIVE)   //Lock every ability but the active one
            {
                ability.Lock();
            }
            else if (newState == EAbilityState.COOLDOWN && ability.FSM.CurrentState.ID == EAbilityState.LOCKED)    //Unlock every ability when the active one goes on cooldown
            {
                ability.Unlock();
            }
        }
    }

}