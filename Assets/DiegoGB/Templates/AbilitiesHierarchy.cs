using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface AbilityBase
{
    //animator

    public float ActiveDuration { get; }
    public float CooldownDuration { get; }

    public float ActiveTimer { get; set; }
    public float CooldownTimer { get; set; }
}

public interface AbilityWithRange : AbilityBase
{
    public float Range { get; }
}

public interface AbilityWithProjectile : AbilityBase
{
    public GameObject ProjectilePrefab { get; }
    public int ProjectileAmount { get; }
    public float ProjectileInterval { get; }
}

public interface AbilityWithTarget : AbilityBase
{
    public GameObject Target { get; }
}

public interface AbilityWithBuff : AbilityBase
{
    public Stats StatModifier { get; }
}


