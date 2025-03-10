using Systems.ServiceLocator;
using Unity.Netcode;
using UnityEngine;

public class SfxSync : NetworkBehaviour
{
    [SerializeField] private AudioSource _buff, _debuff, _heal, _death;

    public override void OnNetworkSpawn()
    {
        if (!IsOwner) return;

        ServiceLocator.Global.Get(out BuffableBehaviour buffable).Get(out DamageableBehaviour damageable);
        buffable.OnBuff += (_, _) => PlayBuff_ClientRpc();
        buffable.OnDebuff += (_, _) => PlayDebuff_ClientRpc();
        damageable.OnHeal += (_) => PlayHeal_ClientRpc();
        damageable.OnDeath += PlayDeath_ClientRpc;
    }

    public override void OnNetworkDespawn()
    {
        if (!IsOwner) return;

        ServiceLocator.Global.Get(out BuffableBehaviour buffable).Get(out DamageableBehaviour damageable);
        buffable.OnBuff -= (_, _) => PlayBuff_ClientRpc();
        buffable.OnDebuff -= (_, _) => PlayDebuff_ClientRpc();
        damageable.OnHeal -= (_) => PlayHeal_ClientRpc();
        damageable.OnDeath -= PlayDeath_ClientRpc;
    }

    [Rpc(SendTo.ClientsAndHost)]
    private void PlayBuff_ClientRpc()
    {
        _buff.Play();
    }

    [Rpc(SendTo.ClientsAndHost)]
    private void PlayDebuff_ClientRpc()
    {
        _debuff.Play();
    }

    [Rpc(SendTo.ClientsAndHost)]
    private void PlayHeal_ClientRpc()
    {
        _heal.Play();
    }

    [Rpc(SendTo.ClientsAndHost)]
    private void PlayDeath_ClientRpc()
    {
        _death.Play();
    }

}