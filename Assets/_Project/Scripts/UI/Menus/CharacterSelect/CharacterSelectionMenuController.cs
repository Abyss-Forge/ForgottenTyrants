using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CharacterPresetXML
{
    public string Name { get; set; }

    public string Race { get; set; }
    public string Class { get; set; }
    public string Armor { get; set; }
    public string Trinket { get; set; }

    public CharacterPresetXML() { }

    public CharacterPresetXML(string name, string race, string characterClass, string armour, string trinket) : this()
    {
        Name = name;

        Race = race;
        Class = characterClass;
        Armor = armour;
        Trinket = trinket;
    }
}

public class CharacterSelectionMenuController : Presettable<CharacterPresetXML>
{
    [Header("Presets")]
    [SerializeField] private PresetSelectorController _presetSelectorController;
    [SerializeField] private Button _savePresetButton, _loadPresetButton;
    [SerializeField] private TMP_InputField _presetNameInputField;
    [SerializeField] private TMP_Text _presetNameErrorText;

    [Header("Selection")]
    [SerializeField] private SelectableCharacterElement _raceSelectableElement, _classSelectableElement, _armorSelectableElement, _trinketSelectableElement;

    [SerializeField] private CharacterElementDatabase<RaceTemplate> _raceDatabase;
    [SerializeField] private CharacterElementDatabase<ClassTemplate> _classDatabase;
    [SerializeField] private CharacterElementDatabase<ArmorTemplate> _armorDatabase;
    [SerializeField] private CharacterElementDatabase<TrinketTemplate> _trinketDatabase;

    public CharacterElementDatabase<RaceTemplate> RaceDatabase => _raceDatabase;
    public CharacterElementDatabase<ClassTemplate> ClassDatabase => _classDatabase;
    public CharacterElementDatabase<ArmorTemplate> ArmorDatabase => _armorDatabase;
    public CharacterElementDatabase<TrinketTemplate> TrinketDatabase => _trinketDatabase;

    public RaceTemplate SelectedRace => _raceDatabase.Elements[_raceSelectableElement.CurrentIndex];
    public ClassTemplate SelectedClass => _classDatabase.Elements[_classSelectableElement.CurrentIndex];
    public ArmorTemplate SelectedArmor => _armorDatabase.Elements[_armorSelectableElement.CurrentIndex];
    public TrinketTemplate SelectedTrinket => _trinketDatabase.Elements[_trinketSelectableElement.CurrentIndex];

    public CharacterSelectionMenuController() : base("CharacterTemplates", "Preset") { }

    void Awake()
    {
        _raceSelectableElement.Initialize(_raceDatabase.Elements);
        _classSelectableElement.Initialize(_classDatabase.Elements);
        _armorSelectableElement.Initialize(_armorDatabase.Elements);
        _trinketSelectableElement.Initialize(_trinketDatabase.Elements);

        _xmlFilePath = "presets.xml";
        _savePresetButton.interactable = false;
        _presetNameErrorText.text = null;
        _presetNameInputField.onValueChanged.AddListener(ValidatePresetName);
        _savePresetButton.onClick.AddListener(SavePreset);
        _loadPresetButton.onClick.AddListener(LoadPreset);
    }

    private void ValidatePresetName(string text)
    {
        bool isNameValid = (text.Length >= 1) && (text.Length <= 20);

        string errorMessage = $"Enter a valid name before saving (1-20 letters)";

        _savePresetButton.interactable = isNameValid;
        _presetNameErrorText.text = isNameValid ? null : errorMessage;
    }

    private void SavePreset()
    {
        string name = _presetNameInputField.text ?? $"Preset {Random.Range(0, 1000)}";
        _presetNameInputField.text = null;

        CharacterPresetXML preset = new(name, SelectedRace.name, SelectedClass.name, SelectedArmor.name, SelectedTrinket.name);
        CreatePreset(preset);
    }

    private void LoadPreset()
    {
        _presetSelectorController.Open(_presets, OnSelectPreset, OnDeletePreset);
    }

    private void OnSelectPreset(CharacterPreset preset)
    {
        _presetSelectorController.Close();

        _raceSelectableElement.Select(preset.PresetModel.Race);
        _classSelectableElement.Select(preset.PresetModel.Class);
        _armorSelectableElement.Select(preset.PresetModel.Armor);
        _trinketSelectableElement.Select(preset.PresetModel.Trinket);
    }

    private void OnDeletePreset(CharacterPreset preset)
    {
        DeletePreset(preset.PresetModel);
    }

}