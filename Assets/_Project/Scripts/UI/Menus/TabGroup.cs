using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(GridLayoutGroup))]
public class TabGroup : MonoBehaviour
{
    [SerializeField] private List<GameObject> _tabPages = new List<GameObject>();
    [SerializeField] private List<MyButton> _tabButtons = new List<MyButton>();
    private MyButton _selectedTabButton;

    void LateUpdate()
    {
        for (int i = 0; i < _tabButtons.Count; i++)
        {
            _tabPages[i].SetActive(_tabButtons[i].IsSelected);
            if (_tabButtons[i].IsSelected)
            {
                _selectedTabButton = _tabButtons[i];
            }
        }
    }

    public void OnTabSelected(MyButton button)
    {
        if (_selectedTabButton != null)
        {
            _selectedTabButton.SetSelected(false);
        }
        _selectedTabButton = button;
        _selectedTabButton.SetSelected(true);

        int index = _tabButtons.IndexOf(button);
        for (int i = 0; i < _tabPages.Count; i++)
        {
            _tabPages[i].SetActive(i == index);
        }
    }
}
