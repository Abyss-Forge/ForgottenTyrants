using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class StatusEffect : MonoBehaviour
{
    [SerializeField] protected string _name;
    [SerializeField] protected float _duration;

    public string Name => _name;
    public float Duration => _duration;

    public virtual void ApplyEffect(Player player) { }
    public virtual void RemoveEffect(Player player) { }

    public IEnumerator EffectRoutine(Player player)
    {
        ApplyEffect(player);
        yield return new WaitForSeconds(_duration);
        RemoveEffect(player);
        Destroy(this);
    }
}
