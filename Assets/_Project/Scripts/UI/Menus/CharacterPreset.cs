using System;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class CharacterPreset : MonoBehaviour, IPointerClickHandler
{
    [SerializeField] private Button _deleteButton;
    [SerializeField] private TextMeshProUGUI _nameText, _raceText, _classText, _armorText, _trinketText;

    public CharacterPresetXML PresetModel { get; private set; }
    public event Action<CharacterPreset> OnDelete, OnSelect;

    public void Initialize(CharacterPresetXML presetModel)
    {
        PresetModel = presetModel;

        _nameText.text = $"{PresetModel.Name}";
        _raceText.text = $"Race: {PresetModel.Race}";
        _classText.text = $"Class: {PresetModel.Class}";
        _armorText.text = $"Armor: {PresetModel.Armor}";
        _trinketText.text = $"Trinket: {PresetModel.Trinket}";
    }

    void OnEnable()
    {
        _deleteButton.onClick.AddListener(WhenDeleted);
    }

    void OnDisable()
    {
        _deleteButton.onClick.RemoveAllListeners();
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        OnSelect?.Invoke(this);
    }

    private void WhenDeleted()
    {
        OnDelete?.Invoke(this);
        Destroy(gameObject);
    }

    public void Initialize()
    {
        throw new NotImplementedException();
    }
}