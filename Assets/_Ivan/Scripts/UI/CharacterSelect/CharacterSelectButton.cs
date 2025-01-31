using UnityEngine;
using UnityEngine.UI;

public class CharacterSelectButton : MonoBehaviour
{
    [SerializeField] private Image _iconImage;
    [SerializeField] private GameObject _disabledOverlay;
    [SerializeField] private Button _button;

    private CharacterSelectController _characterSelect;

    public CharacterTemplate Character { get; private set; }
    public bool IsDisabled { get; private set; }

    void OnEnable()
    {
        _button.onClick.AddListener(Select);
    }

    void OnDisable()
    {
        _button.onClick.AddListener(Select);
    }

    public void Initialize(CharacterSelectController characterSelect, CharacterTemplate character)
    {
        _characterSelect = characterSelect;
        Character = character;
        _iconImage.sprite = character.Icon;
    }

    public void SetDisabled(bool disabled = true)
    {
        IsDisabled = disabled;
        _disabledOverlay.SetActive(disabled);
        _button.interactable = !disabled;
    }

    private void Select()
    {
        _characterSelect.SelectCharacter(Character);
    }

}