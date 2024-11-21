using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class CharacterPreset : MonoBehaviour, IPointerClickHandler
{
    [SerializeField] private Button _deleteButton;
    public TextMeshProUGUI _raceText, _classText, _weaponText, _armourText, _trinketText;
    public CharacterPresetXML _presetModel;

    public event Action<CharacterPreset> OnDelete, OnSelect;

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
        OnSelect.Invoke(this);
    }

    private void WhenDeleted()
    {
        OnDelete.Invoke(this);
        Destroy(gameObject);
    }

}