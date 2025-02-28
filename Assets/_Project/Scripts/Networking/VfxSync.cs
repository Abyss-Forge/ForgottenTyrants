using System.Collections;
using Systems.ServiceLocator;
using Unity.Netcode;
using UnityEngine;

public class VfxSync : NetworkBehaviour
{
    [SerializeField] private ParticleSystem _buff, _debuff, _heal, _death;

    public override void OnNetworkSpawn()
    {
        if (!IsOwner) return;

        ServiceLocator.Global.Get(out BuffableBehaviour buffable).Get(out DamageableBehaviour damageable);
        buffable.OnBuff += (duration, _) => PlayBuff_ClientRpc(duration);
        buffable.OnDebuff += (duration, _) => PlayDebuff_ClientRpc(duration);
        damageable.OnHeal += PlayHeal_ClientRpc;
        damageable.OnDeath += PlayDeath_ClientRpc;
    }

    public override void OnNetworkDespawn()
    {
        if (!IsOwner) return;

        ServiceLocator.Global.Get(out BuffableBehaviour buffable).Get(out DamageableBehaviour damageable);
        buffable.OnBuff -= (duration, _) => PlayBuff_ClientRpc(duration);
        buffable.OnDebuff -= (duration, _) => PlayDebuff_ClientRpc(duration);
        damageable.OnHeal -= PlayHeal_ClientRpc;
        damageable.OnDeath -= PlayDeath_ClientRpc;
    }

    private IEnumerator PlayLooped(ParticleSystem particles, float duration)
    {
        particles.Play();
        yield return new WaitForSeconds(duration);
        particles.Stop();
    }

    [Rpc(SendTo.ClientsAndHost)]
    private void PlayBuff_ClientRpc(float duration)
    {
        StartCoroutine(PlayLooped(_buff, duration));
    }

    [Rpc(SendTo.ClientsAndHost)]
    private void PlayDebuff_ClientRpc(float duration)
    {
        StartCoroutine(PlayLooped(_debuff, duration));
    }

    [Rpc(SendTo.ClientsAndHost)]
    private void PlayHeal_ClientRpc(int amount)
    {
        _heal.Play();
    }

    [Rpc(SendTo.ClientsAndHost)]
    private void PlayDeath_ClientRpc()
    {
        _death.Play();

        Debug.Log("stopping");
        StopAllCoroutines();
    }

}