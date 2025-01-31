using UnityEngine;

public class CharacterSelectionMenuController : MonoBehaviour
{
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

    void Awake()
    {
        _raceSelectableElement.Initialize(_raceDatabase.Elements);
        _classSelectableElement.Initialize(_classDatabase.Elements);
        _armorSelectableElement.Initialize(_armorDatabase.Elements);
        _trinketSelectableElement.Initialize(_trinketDatabase.Elements);
    }

}