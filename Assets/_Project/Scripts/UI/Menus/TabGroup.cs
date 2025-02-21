using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

[Serializable]
class DictItem<K, V>
{
    [SerializeField] public K Key;
    [SerializeField] public V Value;
}

static class DictItemExtensions
{
    public static Dictionary<K, V> ToDictionary<K, V>(this DictItem<K, V>[] items)
    {
        Dictionary<K, V> dict = new();
        foreach (var item in items) dict[item.Key] = item.Value;
        return dict;
    }
}

public class TabGroup : MonoBehaviour
{
    [SerializeField] DictItem<Button, RectTransform>[] _tabs;
    private Dictionary<Button, RectTransform> _tabsDict => _tabs.ToDictionary();

    [SerializeField] private GameObject[] _tabPages;
    [SerializeField] private MyButton[] _tabButtons;
    private MyButton _selectedTabButton;

    void OnEnable()
    {
        for (int i = 0; i < _tabsDict.Count; i++)
        {
            var tab = _tabsDict.ElementAt(i);
            tab.Key.onClick.AddListener(() => HandleButton(tab.Value));
            if (i == 0) continue;
            tab.Key.Select();
            tab.Value.gameObject.SetActive(false);
        }

        foreach (var tab in _tabsDict)
        {
            tab.Key.onClick.AddListener(() => HandleButton(tab.Value));
        }
    }

    void OnDisable()
    {
        foreach (var tab in _tabsDict)
        {
            tab.Key.onClick.RemoveAllListeners();
        }
    }

    private void HandleButton(RectTransform page)
    {
        foreach (var tab in _tabsDict)
        {
            tab.Value.gameObject.SetActive(tab.Value == page);
        }
    }


    void LateUpdate()
    {
        for (int i = 0; i < _tabButtons.Length; i++)
        {
            _tabPages[i].SetActive(_tabButtons[i].IsSelected);
            if (_tabButtons[i].IsSelected) _selectedTabButton = _tabButtons[i];
        }
    }

    public void OnTabSelected(MyButton button)
    {
        if (_selectedTabButton != null) _selectedTabButton.SetSelected(false);
        _selectedTabButton = button;
        _selectedTabButton.SetSelected(true);

        int index = Array.IndexOf(_tabButtons, button);
        for (int i = 0; i < _tabPages.Length; i++)
        {
            _tabPages[i].SetActive(i == index);
        }
    }
}
