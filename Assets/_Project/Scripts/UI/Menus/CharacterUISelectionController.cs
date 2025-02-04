using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;
using Unity.Mathematics;
using System.Linq;

public class CharacterPresetXML
{
    public string Name { get; set; }
    public string Race { get; set; }
    public string Class { get; set; }
    public string Armor { get; set; }
    public string Trinket { get; set; }

    public CharacterPresetXML() { }

    public CharacterPresetXML(string name, string race, string characterClass, string armour, string trinket)
    {
        Name = name;
        Race = race;
        Class = characterClass;
        Armor = armour;
        Trinket = trinket;
    }
}

public class CharacterUISelectionController : Presettable<CharacterPresetXML>
{
    [SerializeField] private PresetSelectorController _presetSelectorController;

    [SerializeField] private List<RaceTemplate> _races;
    [SerializeField] private List<ClassTemplate> _classes;
    private List<WeaponTemplate> _weapons;//dependen de la calse
    [SerializeField] private List<ArmorTemplate> _armours;
    [SerializeField] private List<TrinketTemplate> _trinkets;

    private int _currentRaceIndex = 0;
    private int _currentClassIndex = 0;
    private int _currentArmorIndex = 0;
    private int _currentTrinketIndex = 0;

    [SerializeField] private TMP_Text _raceNameText, _raceStatsText;
    [SerializeField] private TMP_Text _classNameText, _classStatsText;
    [SerializeField] private TMP_Text _weaponNameText, _weaponStatsText;
    [SerializeField] private TMP_Text _armourNameText, _armourStatsText;
    [SerializeField] private TMP_Text _trinketNameText, _trinketStatsText;

    [SerializeField] private Button _savePresetButton, _loadPresetButton;
    [SerializeField] private Button _nextRaceButton, _prevRaceButton;
    [SerializeField] private Button _nextClassButton, _prevClassButton;
    [SerializeField] private Button _nextArmourButton, _prevArmourButton;
    [SerializeField] private Button _nextTrinketButton, _prevTrinketButton;

    public CharacterUISelectionController() : base("CharacterTemplates", "Preset") { }

    public RaceTemplate SelectedRace => _races[_currentRaceIndex];
    public ClassTemplate SelectedClass => _classes[_currentClassIndex];
    public ArmorTemplate SelectedArmor => _armours[_currentArmorIndex];
    public TrinketTemplate SelectedTrinket => _trinkets[_currentTrinketIndex];

    private void Awake()
    {
        _xmlFilePath = "presets.xml";

        ShowRace(_currentRaceIndex);
        ShowClass(_currentClassIndex);
        ShowArmor(_currentArmorIndex);
        ShowTrinket(_currentTrinketIndex);

        _nextRaceButton.onClick.AddListener(NextRace);
        _prevRaceButton.onClick.AddListener(PreviousRace);
        _nextClassButton.onClick.AddListener(NextClass);
        _prevClassButton.onClick.AddListener(PreviousClass);
        _nextArmourButton.onClick.AddListener(NextArmor);
        _prevArmourButton.onClick.AddListener(PreviousArmor);
        _nextTrinketButton.onClick.AddListener(NextTrinket);
        _prevTrinketButton.onClick.AddListener(PreviousTrinket);
        _loadPresetButton.onClick.AddListener(LoadPreset);
        _savePresetButton.onClick.AddListener(SavePreset);
    }


    public void SavePreset()
    {
        CharacterPresetXML preset = new CharacterPresetXML("Name", SelectedRace.name, SelectedClass.name, SelectedArmor.name, SelectedTrinket.name);
        CreatePreset(preset);
    }

    public void LoadPreset()
    {
        _presetSelectorController.Open(_presets, OnSelectPreset, OnDeletePreset);
    }

    private void OnSelectPreset(CharacterPreset preset)
    {
        _presetSelectorController.Close();

        _currentRaceIndex = _races.FindIndex(item => item.name == preset._presetModel.Race);
        ShowRace(_currentRaceIndex);

        _currentClassIndex = _classes.FindIndex(item => item.name == preset._presetModel.Class);
        ShowClass(_currentClassIndex);

        _currentArmorIndex = _armours.FindIndex(item => item.name == preset._presetModel.Armor);
        ShowArmor(_currentArmorIndex);

        _currentTrinketIndex = _trinkets.FindIndex(item => item.name == preset._presetModel.Trinket);
        ShowTrinket(_currentTrinketIndex);
    }

    private void OnDeletePreset(CharacterPreset preset)
    {
        DeletePreset(preset._presetModel);
    }

    #region Interacción con la UI

    private void ShowRace(int index)
    {
        RaceTemplate selectedRace = _races[index];

        _raceNameText.text = selectedRace.name;
        _raceStatsText.text = FormatStats(selectedRace.Stats);
    }

    private void NextRace()
    {
        _currentRaceIndex = (_currentRaceIndex + 1) % _races.Count;
        ShowRace(_currentRaceIndex);
    }

    private void PreviousRace()
    {
        _currentRaceIndex = (_currentRaceIndex - 1 + _races.Count) % _races.Count;
        ShowRace(_currentRaceIndex);
    }

    private void ShowClass(int index)
    {
        ClassTemplate selectedClass = _classes[index];

        _weapons = selectedClass.Weapons.ToList();

        _classNameText.text = selectedClass.name;
        _classStatsText.text = FormatStats(selectedClass.Stats);
    }

    private void NextClass()
    {
        _currentClassIndex = (_currentClassIndex + 1) % _classes.Count;
        ShowClass(_currentClassIndex);
    }

    private void PreviousClass()
    {
        _currentClassIndex = (_currentClassIndex - 1 + _classes.Count) % _classes.Count;
        ShowClass(_currentClassIndex);
    }

    private void ShowWeapon(int index)
    {
        WeaponTemplate selectedWeapon = _weapons[index];

        _weaponNameText.text = selectedWeapon.name;
        _weaponStatsText.text = FormatStats(selectedWeapon.Stats);
    }

    private void ShowArmor(int index)
    {
        ArmorTemplate selectedArmour = _armours[index];

        _armourNameText.text = selectedArmour.name;
        _armourStatsText.text = FormatStats(selectedArmour.Stats);
    }

    private void NextArmor()
    {
        _currentArmorIndex = (_currentArmorIndex + 1) % _armours.Count;
        ShowArmor(_currentArmorIndex);
    }

    private void PreviousArmor()
    {
        _currentArmorIndex = (_currentArmorIndex - 1 + _armours.Count) % _armours.Count;
        ShowArmor(_currentArmorIndex);
    }

    private void ShowTrinket(int index)
    {
        TrinketTemplate selectedTrinket = _trinkets[index];

        _trinketNameText.text = selectedTrinket.name;
        _trinketStatsText.text = FormatStats(selectedTrinket.Stats);
    }

    private void NextTrinket()
    {
        _currentTrinketIndex = (_currentTrinketIndex + 1) % _trinkets.Count;
        ShowTrinket(_currentTrinketIndex);
    }

    private void PreviousTrinket()
    {
        _currentTrinketIndex = (_currentTrinketIndex - 1 + _trinkets.Count) % _trinkets.Count;
        ShowTrinket(_currentTrinketIndex);
    }

    private string FormatStats(Stats stats)
    {
        return $"HP: {stats.Health}\n" +
               $"Physical Damage: {stats.PhysicalDamage}\n" +
               $"Magical Damage: {stats.MagicalDamage}\n" +
               $"Movement Speed: {stats.MovementSpeed}\n" +
               $"Attack Speed: {stats.AttackSpeed}\n" +
               $"Physical Defense: {stats.PhysicalDefense}\n" +
               $"Magical Defense: {stats.MagicalDefense}\n" +
               $"Cd: {stats.CooldownReduction}";
    }

    #endregion
}