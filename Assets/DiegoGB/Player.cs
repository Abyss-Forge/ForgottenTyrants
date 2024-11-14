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


    List<StatusEffect> _statusEffects = new List<StatusEffect>();


    public CharacterRace Race => _race;
    public CharacterClass Class => _class;
    public Weapon Weapon => _weapon;
    public Armour Armour => _armour;
    public Trinket Trinket => _trinket;
    public List<StatusEffect> StatusEffects => _statusEffects;

    protected override void Start()
    {
        base.Start();

    }

    protected override void Update()
    {

    }
    /*public Player(CharacterRaceTemplate selectedRace, CharacterClassTemplate selectedClass, WeaponTemplate selectedWeapon, ArmourTemplate selectedArmour, TrinketTemplate selectedTrinket)
    {
        _race = new CharacterRace(selectedRace);
        _class = new CharacterClass(selectedClass);
        _weapon = new Weapon(selectedWeapon);
        _armour = new Armour(selectedArmour);
        _trinket = new Trinket(selectedTrinket);

        _playerStats = new Stats(0, 0, 0, 0f, 0f, 0, 0, 0f);

        CalculateTotalStats();
    }*/

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
        _stats.Add(_race.Stats);
        _stats.Add(_class.Stats);
        _stats.Add(_weapon.Stats);
        _stats.Add(_armour.Stats);
        _stats.Add(_trinket.Stats);

        Debug.Log($"Total HP: {_stats.Hp}, Physical Damage: {_stats.PhysicalDamage}, " +
                 $"Magical Damage: {_stats.MagicalDamage}, Movement Speed: {_stats.MovementSpeed}, " +
                 $"Attack Speed: {_stats.AttackSpeed}, Physical Defense: {_stats.PhysicalDefense}, " +
                 $"Magical Defense: {_stats.MagicalDefense}, Cooldown Reduction: {_stats.CooldownReduction}");
    }

    public void BuildPlayer(CharacterRaceTemplate selectedRace, CharacterClassTemplate selectedClass, WeaponTemplate selectedWeapon, ArmourTemplate selectedArmour, TrinketTemplate selectedTrinket, string selectedName)
    {
        _name = selectedName;
        _race = new CharacterRace(selectedRace);
        _class = new CharacterClass(selectedClass);
        _weapon = new Weapon(selectedWeapon);
        _armour = new Armour(selectedArmour);
        _trinket = new Trinket(selectedTrinket);

        CalculateTotalStats();
    }

}
