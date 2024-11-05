using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class ItemTemplate : ScriptableObject
{
    [SerializeField] Stats _stats;
    [SerializeField] string _name;

    public Stats Stats => _stats;
    public string Name => _name;
}

[CreateAssetMenu(menuName = "ScriptableObject/CharacterRace")]
public class CharacterRaceTemplate : ItemTemplate
{

}

[CreateAssetMenu(menuName = "ScriptableObject/CharacterClass")]
public class CharacterClassTemplate : ItemTemplate
{

}

[CreateAssetMenu(menuName = "ScriptableObject/Weapon")]
public class WeaponTemplate : ItemTemplate
{

}

[CreateAssetMenu(menuName = "ScriptableObject/Armour")]
public class ArmourTemplate : ItemTemplate
{

}

[CreateAssetMenu(menuName = "ScriptableObject/Trinket")]
public class TrinketTemplate : ItemTemplate
{

}
