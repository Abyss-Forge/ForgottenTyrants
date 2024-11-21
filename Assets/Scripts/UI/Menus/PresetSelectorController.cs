using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PresetSelectorController : MonoBehaviour
{
    [SerializeField] private List<CharacterPreset> _presets;
    [SerializeField] private Button _closeButton;
    [SerializeField] private GameObject _menu, _list, _presetPrefab;

    void Awake()
    {
        _closeButton.onClick.AddListener(Close);
    }

    private void Close()
    {
        _menu.SetActive(false);

        foreach (Transform child in _list.transform)
        {
            Destroy(child.gameObject);
        }
    }

    public void Open(List<CharacterPresetXML> presets, Action<CharacterPreset> OnSelect, Action<CharacterPreset> OnDelete)
    {
        _menu.SetActive(true);

        foreach (var item in presets)
        {
            GameObject instance = Instantiate(_presetPrefab, Vector2.zero, Quaternion.identity, _list.GetComponent<RectTransform>());
            CharacterPreset preset = instance.GetComponent<CharacterPreset>();

            preset._raceText.text = $"Race: {item.Race.name}";
            preset._classText.text = $"Class: {item.Class.name}";
            preset._weaponText.text = $"Weapon: {item.Weapon.name}";
            preset._armourText.text = $"Armour: {item.Armour.name}";
            preset._trinketText.text = $"Trinket: {item.Trinket.name}";

            preset._presetModel = item;
            preset.OnSelect += OnDelete;
            preset.OnDelete += OnDelete;

            _presets.Add(preset);
        }
    }

}