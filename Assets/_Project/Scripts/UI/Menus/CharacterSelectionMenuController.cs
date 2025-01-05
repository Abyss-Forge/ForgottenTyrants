using System;
using System.Collections.Generic;
using UnityEngine;

public class CharacterSelectionMenuController : MonoBehaviour
{
    [SerializeField] private SelectableCharacterElement _raceSelectableElement, _classSelectableElement, _armorSelectableElement, _trinketSelectableElement;

    [SerializeField] private CharacterElementDatabase<RaceTemplate> _raceDatabase;
    [SerializeField] private CharacterElementDatabase<ClassTemplate> _classDatabase;
    [SerializeField] private CharacterElementDatabase<ArmorTemplate> _armorDatabase;
    [SerializeField] private CharacterElementDatabase<TrinketTemplate> _trinketDatabase;

    private void Awake()
    {
        _raceSelectableElement.Initialize(_raceDatabase.Elements);
        _classSelectableElement.Initialize(_classDatabase.Elements);
        _armorSelectableElement.Initialize(_armorDatabase.Elements);
        _trinketSelectableElement.Initialize(_trinketDatabase.Elements);
    }

    public void Ready(ulong clientId)
    {
        RaceTemplate selectedRace = _raceDatabase.Elements[_raceSelectableElement.CurrentIndex];
        ClassTemplate selectedClass = _classDatabase.Elements[_classSelectableElement.CurrentIndex];
        ArmorTemplate selectedArmor = _armorDatabase.Elements[_armorSelectableElement.CurrentIndex];
        TrinketTemplate selectedTrinket = _trinketDatabase.Elements[_trinketSelectableElement.CurrentIndex];

        HostManager.Instance.SetCharacterBuild(clientId, selectedRace, selectedClass, selectedArmor, selectedTrinket);
    }

}