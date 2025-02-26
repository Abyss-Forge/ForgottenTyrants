using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PresetSelectorController : MonoBehaviour
{
    [SerializeField] private Button _closeButton;
    [SerializeField] private RectTransform _menu, _list;
    [SerializeField] private CharacterPreset _presetPrefab;

    void OnEnable()
    {
        _closeButton.onClick.AddListener(Close);
    }

    void OnDisable()
    {
        _closeButton.onClick.RemoveAllListeners();
    }

    public void Close()
    {
        _menu.gameObject.SetActive(false);

        foreach (Transform child in _list)
        {
            Destroy(child.gameObject);
        }
    }

    public void Open(List<CharacterPresetXML> presets, Action<CharacterPreset> OnSelect, Action<CharacterPreset> OnDelete)
    {
        _menu.gameObject.SetActive(true);

        foreach (var item in presets)
        {
            CharacterPreset instance = Instantiate(_presetPrefab, Vector2.zero, Quaternion.identity, _list);
            instance.Initialize(item);
            instance.OnSelect += OnSelect;
            instance.OnDelete += OnDelete;
        }
    }

}