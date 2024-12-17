using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SelectableCharacterElement : MonoBehaviour
{
    [SerializeField] private TMP_Text _titleText, _elementNameText;
    [SerializeField] private Button _nextButton, _previousButton;

    private void OnEnable()
    {

    }

    private void OnDisable()
    {
        _nextButton.onClick.RemoveAllListeners();
        _previousButton.onClick.RemoveAllListeners();
    }

}
