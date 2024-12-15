using System.Collections;
using System.Collections.Generic;
using UnityEditor;
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
        //BuildPlayer();
        _currentHp = _stats.Hp;

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
        _stats.Add(Race.Stats);
        _stats.Add(Class.Stats);
        _stats.Add(Weapon.Stats);
        _stats.Add(Armour.Stats);
        _stats.Add(Trinket.Stats);

        Debug.Log($"Total HP: {_stats.Hp}, Physical Damage: {_stats.PhysicalDamage}, " +
                 $"Magical Damage: {_stats.MagicalDamage}, Movement Speed: {_stats.MovementSpeed}, " +
                 $"Attack Speed: {_stats.AttackSpeed}, Physical Defense: {_stats.PhysicalDefense}, " +
                 $"Magical Defense: {_stats.MagicalDefense}, Cooldown Reduction: {_stats.CooldownReduction}");
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

    public int GetCurrentHp()
    {
        return _currentHp;
    }

#if UNITY_EDITOR

    [SerializeField] BossController bossController;
    public void GenerateColliderStructure()
    {

        bossController.TakeDamage(this, 20);
    }

    [CustomEditor(typeof(Player))]
    public class RootModelGeneratorEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();

            Player generator = (Player)target;

            if (GUILayout.Button("Generate Collider Structure"))
            {
                generator.GenerateColliderStructure();
            }


            GUI.enabled = true;
        }
    }

#endif

}


