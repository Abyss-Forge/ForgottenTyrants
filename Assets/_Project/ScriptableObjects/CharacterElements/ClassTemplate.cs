using UnityEngine;

[CreateAssetMenu(menuName = "ScriptableObject/CharacterElement/Class")]
public class ClassTemplate : CharacterElementTemplate
{
    [field: SerializeField] public AbilityTemplate[] Abilities { get; protected set; }
    [field: SerializeField] public WeaponTemplate[] Weapons { get; protected set; }

}