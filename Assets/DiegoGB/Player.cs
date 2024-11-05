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
        CalculateTotalStats();
    }

    protected override void Update()
    {

    }

    private void CalculateTotalStats()
    {
        _playerStats.Add(_race.RaceStats);
        _playerStats.Add(_class.ClassStats);
        _playerStats.Add(_weapon.WeaponStats);
        _playerStats.Add(_armour.ArmourStats);

        Debug.Log($"Total HP: {_playerStats.Hp}, Physical Damage: {_playerStats.PhysicalDamage}, " +
                 $"Magical Damage: {_playerStats.MagicalDamage}, Movement Speed: {_playerStats.MovementSpeed}, " +
                 $"Attack Speed: {_playerStats.AttackSpeed}, Physical Defense: {_playerStats.PhysicalDefense}, " +
                 $"Magical Defense: {_playerStats.MagicalDefense}, Cooldown Reduction: {_playerStats.CooldownReduction}");
    }
}
