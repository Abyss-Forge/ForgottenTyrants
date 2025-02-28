using Systems.EventBus;
using Unity.Netcode;
using UnityEngine;

[RequireComponent(typeof(Animator))]
public class AnimatorEnabledStateSync : NetworkBehaviour
{
    Animator _animator;

    private EventBinding<PlayerDeathEvent> _playerDeathEventBinding;
    private EventBinding<PlayerRespawnEvent> _playerRespawnEventBinding;

    bool isLocal = false;

    public void Initialize()
    {
        isLocal = true;

        _playerDeathEventBinding = new EventBinding<PlayerDeathEvent>(HandleDeath);
        EventBus<PlayerDeathEvent>.Register(_playerDeathEventBinding);

        _playerRespawnEventBinding = new EventBinding<PlayerRespawnEvent>(HandleRespawn);
        EventBus<PlayerRespawnEvent>.Register(_playerRespawnEventBinding);
        Debug.Log("spawn model offline" + gameObject.name + IsOwner);
    }

    void Awake()
    {
        _animator = GetComponent<Animator>();
    }

    public override void OnNetworkSpawn()
    {
        Debug.Log("spawn model" + gameObject.name + IsOwner);
        if (!IsOwner) return;

        _playerDeathEventBinding = new EventBinding<PlayerDeathEvent>(HandleDeath);
        EventBus<PlayerDeathEvent>.Register(_playerDeathEventBinding);

        _playerRespawnEventBinding = new EventBinding<PlayerRespawnEvent>(HandleRespawn);
        EventBus<PlayerRespawnEvent>.Register(_playerRespawnEventBinding);
    }

    public override void OnNetworkDespawn()
    {
        if (!IsOwner) return;

        EventBus<PlayerDeathEvent>.Deregister(_playerDeathEventBinding);
        EventBus<PlayerRespawnEvent>.Deregister(_playerRespawnEventBinding);
    }

    private void HandleDeath()
    {
        if (isLocal)
        {
            SetAnimatorEnabled(false);
        }
        else
        {
            SetAnimatorEnabled_ClientRpc(false);
        }
    }

    private void HandleRespawn()
    {
        if (isLocal)
        {
            SetAnimatorEnabled(true);
        }
        else
        {
            SetAnimatorEnabled_ClientRpc(true);
        }
    }

    [Rpc(SendTo.ClientsAndHost)]
    private void SetAnimatorEnabled_ClientRpc(bool enabled)
    {
        SetAnimatorEnabled(enabled);
    }

    private void SetAnimatorEnabled(bool enabled)
    {
        _animator.enabled = enabled;

        foreach (var rb in GetComponentsInChildren<Rigidbody>())
        {
            rb.isKinematic = enabled;
        }
    }

}