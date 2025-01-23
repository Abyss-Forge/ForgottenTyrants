using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TeamSelectElement : MonoBehaviour
{
    [SerializeField] private TMP_Text _playerCountText;
    [SerializeField] private Button _button;

    private CharacterSelectController _characterSelect;

    public Team Team { get; private set; }
    public bool IsDisabled { get; private set; }

    void OnEnable()
    {
        _button.onClick.AddListener(Select);
    }

    void OnDisable()
    {
        _button.onClick.AddListener(Select);
    }

    public void SetTeam(CharacterSelectController characterSelect, Team team)
    {
        _characterSelect = characterSelect;

        Team = team;

        UpdateText();
    }

    public void SetDisabled(bool disabled = true)
    {
        IsDisabled = disabled;
        _button.interactable = !disabled;

        UpdateText();
    }

    private void Select()
    {
        _characterSelect.SelectTeam(Team);

        UpdateText();
    }

    private void UpdateText()
    {
        _playerCountText.text = $"{Team.CurrentPlayerCount} / {Team.Size}";
    }

}