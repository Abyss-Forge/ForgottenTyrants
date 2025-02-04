using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEditor;
using UnityEngine;

public class Player : Entity
{
    private List<StatusEffect> _statusEffects = new();
    public List<StatusEffect> StatusEffects => _statusEffects;

    public ClientData _playerData;

    void Awake()
    {
        _playerData = HostManager.Instance.GetMyClientData();
        Debug.Log("Team" + _playerData.TeamId);
        CalculateTotalStats();
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
        _baseStats.Add(_playerData.Race.Stats);
        _baseStats.Add(_playerData.Class.Stats);
        _baseStats.Add(_playerData.Armor.Stats);
        _baseStats.Add(_playerData.Trinket.Stats);

        Debug.Log($"Total HP: {_baseStats.Health}, Physical Damage: {_baseStats.PhysicalDamage}, " +
                 $"Magical Damage: {_baseStats.MagicalDamage}, Movement Speed: {_baseStats.MovementSpeed}, " +
                 $"Attack Speed: {_baseStats.AttackSpeed}, Physical Defense: {_baseStats.PhysicalDefense}, " +
                 $"Magical Defense: {_baseStats.MagicalDefense}, Cooldown Reduction: {_baseStats.CooldownReduction}");
    }


    //  Diego me cago en tu raza
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