using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CharacterUISelectionController : MonoBehaviour
{
    [SerializeField] private List<CharacterRaceTemplate> races;
    [SerializeField] private List<CharacterClassTemplate> classes;
    [SerializeField] private List<WeaponTemplate> weapons;
    [SerializeField] private List<ArmourTemplate> armours;
    [SerializeField] private List<TrinketTemplate> trinkets;


    private int currentRaceIndex = 0;
    private int currentClassIndex = 0;
    private int currentWeaponIndex = 0;
    private int currentArmourIndex = 0;
    private int currentTrinketIndex = 0;


    [SerializeField] private TMP_Text raceNameText;
    [SerializeField] private TMP_Text raceStatsText;
    [SerializeField] private Player player;

    private void Start()
    {
        if (races.Count > 0)
        {
            ShowRace(currentRaceIndex);
        }
    }

    private void ShowRace(int index)
    {
        CharacterRaceTemplate selectedRace = races[index];

        raceNameText.text = selectedRace.Name;
        raceStatsText.text = FormatStats(selectedRace.Stats);
    }
    private string FormatStats(Stats stats)
    {
        return $"HP: {stats.Hp}\n" +
               $"Physical Damage: {stats.PhysicalDamage}\n" +
               $"Magical Damage: {stats.MagicalDamage}\n" +
               $"Movement Speed: {stats.MovementSpeed}\n" +
               $"Attack Speed: {stats.AttackSpeed}\n" +
               $"Physical Defense: {stats.PhysicalDefense}\n" +
               $"Magical Defense: {stats.MagicalDefense}\n" +
               $"Cooldown Reduction: {stats.CooldownReduction}";
    }
    public void NextRace()
    {
        currentRaceIndex = (currentRaceIndex + 1) % races.Count;
        ShowRace(currentRaceIndex);
    }
    public void PreviousRace()
    {
        currentRaceIndex = (currentRaceIndex - 1 + races.Count) % races.Count;
        ShowRace(currentRaceIndex);
    }
    public CharacterRaceTemplate GetSelectedRace()
    {
        return races[currentRaceIndex];
    }
    public CharacterClassTemplate GetSelectedClass()
    {
        return classes[currentClassIndex];
    }
    public WeaponTemplate GetSelectedWeapon()
    {
        return weapons[currentWeaponIndex];
    }
    public ArmourTemplate GetSelectedArmour()
    {
        return armours[currentArmourIndex];
    }
    public TrinketTemplate GetSelectedTrinket()
    {
        return trinkets[currentTrinketIndex];
    }
    public void ConfirmSelection()
    {
        CharacterRaceTemplate selectedRace = GetSelectedRace();
        CharacterClassTemplate selectedClass = GetSelectedClass();
        WeaponTemplate selectedWeapon = GetSelectedWeapon();
        ArmourTemplate selectedArmour = GetSelectedArmour();
        TrinketTemplate selectedTrinket = GetSelectedTrinket();
    }
}
