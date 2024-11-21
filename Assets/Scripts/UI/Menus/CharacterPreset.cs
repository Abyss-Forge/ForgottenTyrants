using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class CharacterPreset : MonoBehaviour, IPointerClickHandler
{
    public CharacterPresetXML _presetModel;
    [SerializeField] private Button _deleteButton;
    public TextMeshProUGUI _raceText, _classText, _weaponText, _armourText, _trinketText;

    public event Action<CharacterPreset> OnDelete, OnSelect;

    void OnEnable()
    {
        _deleteButton.onClick.AddListener(() => OnDelete.Invoke(this));
    }

    void OnDisable()
    {
        _deleteButton.onClick.RemoveAllListeners();
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        OnSelect.Invoke(this);
    }

}