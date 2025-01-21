using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ElementalType
{
    PHYSIC, MAGIC
}

public class DamageInfo : MonoBehaviour
{

    /* [Serializable]
     public struct Damage
     {
         public ElementalType DamageType;
         public float DamageAmount;
     }*/

    public ElementalType ElementalType { get; }
    public float Damage { get; }
    public int TeamId { get; }

    public DamageInfo(int teamId, float damage, ElementalType elementalType)
    {
        TeamId = teamId;
        Damage = damage;
        ElementalType = elementalType;
    }

}