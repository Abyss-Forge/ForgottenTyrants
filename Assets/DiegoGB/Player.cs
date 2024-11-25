using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : Entity
{
    [field: SerializeField] public CharacterRace Race { get; set; }
    [field: SerializeField] public CharacterClass Class { get; set; }
    [field: SerializeField] public Weapon Weapon { get; set; }
    [field: SerializeField] public Armour Armour { get; set; }
    [field: SerializeField] public Trinket Trinket { get; set; }

    private List<StatusEffect> _statusEffects = new();
    public List<StatusEffect> StatusEffects => _statusEffects;

    protected override void Start()
    {
        base.Start();

    }

    protected override void Update()
    {

    }

    public void AddStatusEffect(StatusEffect statusEffect)
    {
        if (statusEffect != null)
        {
            _statusEffects.Add(statusEffect);
            Debug.Log($"Added StatusEffect: {statusEffect.Name}");
        }
    }

    public void RemoveStatusEffect(StatusEffect statusEffect)
    {
        if (_statusEffects.Contains(statusEffect))
        {
            _statusEffects.Remove(statusEffect);
            Debug.Log($"Removed StatusEffect: {statusEffect.Name}");
        }
    }

    private void CalculateTotalStats()
    {
        _baseStats.Add(Race.Stats);
        _baseStats.Add(Class.Stats);
        _baseStats.Add(Weapon.Stats);
        _baseStats.Add(Armour.Stats);
        _baseStats.Add(Trinket.Stats);

        Debug.Log($"Total HP: {_baseStats.Hp}, Physical Damage: {_baseStats.PhysicalDamage}, " +
                 $"Magical Damage: {_baseStats.MagicalDamage}, Movement Speed: {_baseStats.MovementSpeed}, " +
                 $"Attack Speed: {_baseStats.AttackSpeed}, Physical Defense: {_baseStats.PhysicalDefense}, " +
                 $"Magical Defense: {_baseStats.MagicalDefense}, Cooldown Reduction: {_baseStats.CooldownReduction}");
    }

    public void BuildPlayer(CharacterRaceTemplate selectedRace, CharacterClassTemplate selectedClass, WeaponTemplate selectedWeapon, ArmourTemplate selectedArmour, TrinketTemplate selectedTrinket, string selectedName)
    {
        _name = selectedName;
        Race = new CharacterRace(selectedRace);
        Class = new CharacterClass(selectedClass);
        Weapon = new Weapon(selectedWeapon);
        Armour = new Armour(selectedArmour);
        Trinket = new Trinket(selectedTrinket);

        CalculateTotalStats();
    }

}
