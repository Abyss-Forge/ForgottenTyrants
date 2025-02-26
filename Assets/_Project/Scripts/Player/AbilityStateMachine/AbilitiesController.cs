using System.Collections.Generic;
using Systems.EventBus;
using Systems.GameManagers;
using Systems.ServiceLocator;
using UnityEngine;
using Utils.Extensions;

public class AbilitiesController : MonoBehaviour
{
    [SerializeField] RectTransform _abilitiesIconsHolder;
    [SerializeField] Transform _abilitiesHolder;

    private List<AbilityStateMachine> _abilities;

    private EventBinding<PlayerDeathEvent> _playerDeathEventBinding;
    private EventBinding<PlayerRespawnEvent> _playerRespawnEventBinding;

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
        _playerDeathEventBinding = new EventBinding<PlayerDeathEvent>(HandleDeath);
        EventBus<PlayerDeathEvent>.Register(_playerDeathEventBinding);

        _playerRespawnEventBinding = new EventBinding<PlayerRespawnEvent>(HandleRespawn);
        EventBus<PlayerRespawnEvent>.Register(_playerRespawnEventBinding);

        RegisterInputs(true);
    }

    void OnDisable()
    {
        EventBus<PlayerDeathEvent>.Deregister(_playerDeathEventBinding);
        EventBus<PlayerRespawnEvent>.Deregister(_playerRespawnEventBinding);

        RegisterInputs(false);
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
                        //necesitamos que se suscriban a los eventos cuando hayan terminado de inicializarse,
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

    private void HandleDeath()
    {
        RegisterInputs(false);
    }

    private void HandleRespawn()
    {
        RegisterInputs(true);
    }

    private void RegisterInputs(bool register)
    {
        MyInputManager.Instance.Subscribe(EInputAction.CLASS_ABILITY_1, _abilities[0].OnTriggered, register);
        MyInputManager.Instance.Subscribe(EInputAction.CLASS_ABILITY_2, _abilities[1].OnTriggered, register);
        MyInputManager.Instance.Subscribe(EInputAction.CLASS_ABILITY_3, _abilities[2].OnTriggered, register);
        MyInputManager.Instance.Subscribe(EInputAction.CLASS_ABILITY_4, _abilities[3].OnTriggered, register);
    }

}