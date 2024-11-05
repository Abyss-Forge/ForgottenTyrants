using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : Entity
{
    [SerializeField] CharacterRace _race;
    [SerializeField] CharacterClass _class;
    [SerializeField] Weapon _weapon;
    [SerializeField] Armour _armour;
    [SerializeField] Trinket _trinket;
    Stats _playerStats = new Stats(0, 0, 0, 0f, 0f, 0, 0, 0f);

    protected override void Start()
    {
        base.Start();
    }

    public void SetRace(CharacterRace race)
    {
        _race = race;
    }

    public void SetClass(CharacterClass characterClass)
    {
        _class = characterClass;
    }

    public void SetWeapon(Weapon weapon)
    {
        _weapon = weapon;
    }

    public void SetArmour(Armour armour)
    {
        _armour = armour;
    }

    public void SetTrinket(Trinket trinket)
    {
        _trinket = trinket;
    }

    private void CalculateTotalStats()
    {
        _playerStats.Add(_race.Stats);
        _playerStats.Add(_class.Stats);
        _playerStats.Add(_weapon.Stats);
        _playerStats.Add(_armour.Stats);
        _playerStats.Add(_trinket.Stats);

        Debug.Log($"Total HP: {_playerStats.Hp}, Physical Damage: {_playerStats.PhysicalDamage}, " +
                 $"Magical Damage: {_playerStats.MagicalDamage}, Movement Speed: {_playerStats.MovementSpeed}, " +
                 $"Attack Speed: {_playerStats.AttackSpeed}, Physical Defense: {_playerStats.PhysicalDefense}, " +
                 $"Magical Defense: {_playerStats.MagicalDefense}, Cooldown Reduction: {_playerStats.CooldownReduction}");
    }
}
