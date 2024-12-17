using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class Player : Entity
{
    [field: SerializeField] public RaceTemplate Race { get; set; }
    [field: SerializeField] public ClassTemplate Class { get; set; }
    [field: SerializeField] public WeaponTemplate Weapon { get; set; }
    [field: SerializeField] public ArmourTemplate Armour { get; set; }
    [field: SerializeField] public TrinketTemplate Trinket { get; set; }

    private List<StatusEffect> _statusEffects = new();
    public List<StatusEffect> StatusEffects => _statusEffects;

    protected override void Start()
    {
        base.Start();
        //BuildPlayer();
        _currentHp = BaseStats.Hp;

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

    public void BuildPlayer(RaceTemplate selectedRace, ClassTemplate selectedClass, WeaponTemplate selectedWeapon, ArmourTemplate selectedArmour, TrinketTemplate selectedTrinket, string selectedName)
    {
        _name = selectedName;
        Race = selectedRace;
        Class = selectedClass;
        Weapon = selectedWeapon;
        Armour = selectedArmour;
        Trinket = selectedTrinket;

        CalculateTotalStats();
    }

    public int GetCurrentHp()
    {
        return _currentHp;
    }

#if UNITY_EDITOR

    [SerializeField] BossController bossController;
    public void Test()
    {

        bossController.TakeDamage(this, 20);
    }

    [CustomEditor(typeof(Player))]
    public class TestEditorButton : Editor
    {
        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();

            Player generator = (Player)target;

            if (GUILayout.Button("Test"))
            {
                generator.Test();
            }


            GUI.enabled = true;
        }
    }

#endif

}


