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
    public string Weapon { get; set; }
    public string Armour { get; set; }
    public string Trinket { get; set; }

    public CharacterPresetXML() { }

    public CharacterPresetXML(string name, string race, string characterClass, string weapon, string armour, string trinket)
    {
        Name = name;
        Race = race;
        Class = characterClass;
        Weapon = weapon;
        Armour = armour;
        Trinket = trinket;
    }
}

public class CharacterUISelectionController : Presettable<CharacterPresetXML>
{
    [SerializeField] private GameObject _playerPrefab;
    [SerializeField] private PresetSelectorController _presetSelectorController;

    [SerializeField] private List<RaceTemplate> _races;
    [SerializeField] private List<ClassTemplate> _classes;
    private List<WeaponTemplate> _weapons;//dependen de la calse
    [SerializeField] private List<ArmourTemplate> _armours;
    [SerializeField] private List<TrinketTemplate> _trinkets;

    private int _currentRaceIndex = 0;
    private int _currentClassIndex = 0;
    private int _currentWeaponIndex = 0;
    private int _currentArmourIndex = 0;
    private int _currentTrinketIndex = 0;

    [SerializeField] private TMP_Text _raceNameText, _raceStatsText;
    [SerializeField] private TMP_Text _classNameText, _classStatsText;
    [SerializeField] private TMP_Text _weaponNameText, _weaponStatsText;
    [SerializeField] private TMP_Text _armourNameText, _armourStatsText;
    [SerializeField] private TMP_Text _trinketNameText, _trinketStatsText;

    [SerializeField] private Button _savePresetButton, _loadPresetButton;
    [SerializeField] private Button _confirmButton, _cancelButton;
    [SerializeField] private Button _nextRaceButton, _prevRaceButton;
    [SerializeField] private Button _nextClassButton, _prevClassButton;
    [SerializeField] private Button _nextWeaponButton, _prevWeaponButton;
    [SerializeField] private Button _nextArmourButton, _prevArmourButton;
    [SerializeField] private Button _nextTrinketButton, _prevTrinketButton;

    public CharacterUISelectionController() : base("CharacterTemplates", "Preset") { }

    public RaceTemplate SelectedRace => _races[_currentRaceIndex];
    public ClassTemplate SelectedClass => _classes[_currentClassIndex];
    public WeaponTemplate SelectedWeapon => _weapons[_currentWeaponIndex];
    public ArmourTemplate SelectedArmour => _armours[_currentArmourIndex];
    public TrinketTemplate SelectedTrinket => _trinkets[_currentTrinketIndex];

    private void Awake()
    {
        _xmlFilePath = "presets.xml";

        ShowRace(_currentRaceIndex);
        ShowClass(_currentClassIndex);
        ShowWeapon(_currentWeaponIndex);
        ShowArmour(_currentArmourIndex);
        ShowTrinket(_currentTrinketIndex);

        _nextRaceButton.onClick.AddListener(NextRace);
        _prevRaceButton.onClick.AddListener(PreviousRace);
        _nextClassButton.onClick.AddListener(NextClass);
        _prevClassButton.onClick.AddListener(PreviousClass);
        _nextWeaponButton.onClick.AddListener(NextWeapon);
        _prevWeaponButton.onClick.AddListener(PreviousWeapon);
        _nextArmourButton.onClick.AddListener(NextArmour);
        _prevArmourButton.onClick.AddListener(PreviousArmour);
        _nextTrinketButton.onClick.AddListener(NextTrinket);
        _prevTrinketButton.onClick.AddListener(PreviousTrinket);
        _confirmButton.onClick.AddListener(ConfirmSelection);
        _cancelButton.onClick.AddListener(Back);
        _loadPresetButton.onClick.AddListener(LoadPreset);
        _savePresetButton.onClick.AddListener(SavePreset);
    }

    public void ConfirmSelection()
    {
        GameObject playerInstance = Instantiate(_playerPrefab, Vector3.zero, quaternion.identity);

        Player playerScript = playerInstance.GetComponentInChildren<Player>();
        playerScript?.BuildPlayer(SelectedRace, SelectedClass, SelectedWeapon, SelectedArmour, SelectedTrinket, "YOU YOU");

        DontDestroyOnLoad(playerInstance);
        SceneManager.LoadScene(ForgottenTyrants.Scene.PruebasDiego);
    }

    public void Back()
    {

    }

    public void SavePreset()
    {
        CharacterPresetXML preset = new CharacterPresetXML("Name", SelectedRace.name, SelectedClass.name, SelectedWeapon.name, SelectedArmour.name, SelectedTrinket.name);
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

        _currentWeaponIndex = _weapons.FindIndex(item => item.name == preset._presetModel.Weapon);
        ShowWeapon(_currentWeaponIndex);

        _currentArmourIndex = _armours.FindIndex(item => item.name == preset._presetModel.Armour);
        ShowArmour(_currentArmourIndex);

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

    private void NextWeapon()
    {
        _currentWeaponIndex = (_currentWeaponIndex + 1) % _weapons.Count;
        ShowWeapon(_currentWeaponIndex);
    }

    private void PreviousWeapon()
    {
        _currentWeaponIndex = (_currentWeaponIndex - 1 + _weapons.Count) % _weapons.Count;
        ShowWeapon(_currentWeaponIndex);
    }

    private void ShowArmour(int index)
    {
        ArmourTemplate selectedArmour = _armours[index];

        _armourNameText.text = selectedArmour.name;
        _armourStatsText.text = FormatStats(selectedArmour.Stats);
    }

    private void NextArmour()
    {
        _currentArmourIndex = (_currentArmourIndex + 1) % _armours.Count;
        ShowArmour(_currentArmourIndex);
    }

    private void PreviousArmour()
    {
        _currentArmourIndex = (_currentArmourIndex - 1 + _armours.Count) % _armours.Count;
        ShowArmour(_currentArmourIndex);
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
        return $"HP: {stats.Hp}\n" +
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