using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IAbilityBase
{
    //animator

    public float ActiveDuration { get; }
    public float CooldownDuration { get; }

    public float ActiveTimer { get; set; }
    public float CooldownTimer { get; set; }
}

public interface IAbilityWithRange : IAbilityBase
{
    public float Range { get; }

    void OnDrawGizmos();
}

public interface IAbilityWithProjectile : IAbilityBase
{
    public GameObject ProjectilePrefab { get; }
    public int ProjectileAmount { get; }
    public float ProjectileInterval { get; }
}

public interface IAbilityWithTarget : IAbilityBase
{
    public GameObject Target { get; }
}

public interface IAbilityWithBuff : IAbilityBase
{
    public Stats StatModifier { get; }
}


