using Systems.EventBus;
using Unity.Netcode;
using UnityEngine;

[RequireComponent(typeof(Animator))]
public class AnimatorEnabledSync : NetworkBehaviour
{
    Animator _animator;

    private EventBinding<PlayerDeathEvent> _playerDeathEventBinding;
    private EventBinding<PlayerRespawnEvent> _playerRespawnEventBinding;

    void Awake()
    {
        _animator = GetComponent<Animator>();
    }

    void OnEnable()
    {
        _playerDeathEventBinding = new EventBinding<PlayerDeathEvent>(HandleDeath);
        EventBus<PlayerDeathEvent>.Register(_playerDeathEventBinding);

        _playerRespawnEventBinding = new EventBinding<PlayerRespawnEvent>(HandleRespawn);
        EventBus<PlayerRespawnEvent>.Register(_playerRespawnEventBinding);
    }

    void OnDisable()
    {
        EventBus<PlayerDeathEvent>.Deregister(_playerDeathEventBinding);
        EventBus<PlayerRespawnEvent>.Deregister(_playerRespawnEventBinding);
    }

    private void HandleDeath()
    {
        SetAnimatorEnabled_ClientRpc(false);
    }

    private void HandleRespawn()
    {
        SetAnimatorEnabled_ClientRpc(true);
    }

    [Rpc(SendTo.ClientsAndHost)]
    private void SetAnimatorEnabled_ClientRpc(bool enabled)
    {
        _animator.enabled = enabled;
    }

}