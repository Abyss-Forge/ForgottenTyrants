using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SelectableCharacterElement : MonoBehaviour
{
    [SerializeField] private TMP_Text _elementNameText;
    [SerializeField] private Button _nextButton, _previousButton;

    private List<CharacterElementTemplate> _elements;
    private int _currentIndex;
    public int CurrentIndex => _currentIndex;
    public event Action<int> OnChange;

    public void Initialize(CharacterElementTemplate[] elementArray) => Initialize(new List<CharacterElementTemplate>(elementArray));
    public void Initialize(List<CharacterElementTemplate> elementList)
    {
        _elements = elementList;
        _currentIndex = 0;

        InvokeOnChange();
    }

    void OnEnable()
    {
        _nextButton.onClick.AddListener(Next);
        _previousButton.onClick.AddListener(Previous);
    }

    void OnDisable()
    {
        _nextButton.onClick.RemoveAllListeners();
        _previousButton.onClick.RemoveAllListeners();
    }

    private void Next()
    {
        _currentIndex = (_currentIndex + 1) % _elements.Count;

        InvokeOnChange();
    }

    private void Previous()
    {
        _currentIndex = (_currentIndex - 1 + _elements.Count) % _elements.Count;

        InvokeOnChange();
    }

    private void InvokeOnChange()
    {
        _elementNameText.text = _elements[_currentIndex].name;

        OnChange?.Invoke(_currentIndex);
    }

}
