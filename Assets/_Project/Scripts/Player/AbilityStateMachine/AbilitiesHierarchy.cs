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
    float Range { get; }
}

public interface IAbilityWithProjectile : IAbilityBase
{
    GameObject ProjectilePrefab { get; }
    int ProjectileAmount { get; }
    float ProjectileThreshold { get; }
    float LaunchForce { get; }
}

public interface IAbilityWithTarget : IAbilityBase
{
    GameObject Target { get; }
}

public interface IAbilityWithBuff : IAbilityBase
{
    Stats StatModifier { get; }
}

public interface IAbilityWithDotTick : IAbilityBase
{
    float DotThreshold { get; }
}
