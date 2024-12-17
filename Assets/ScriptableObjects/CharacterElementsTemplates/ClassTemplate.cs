using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "ScriptableObject/CharacterElement/Class")]
public class ClassTemplate : CharacterElementTemplate
{
    [field: SerializeField] public AbilityStateMachine[] Abilities { get; protected set; }
    [field: SerializeField] public WeaponTemplate[] Weapons { get; protected set; }

}