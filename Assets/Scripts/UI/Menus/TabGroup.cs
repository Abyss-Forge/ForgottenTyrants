using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(GridLayoutGroup))]
public class TabGroup : MonoBehaviour
{
    public List<GameObject> tabPages = new List<GameObject>();
    public List<MyButton> tabButtons = new List<MyButton>();
    private MyButton selectedTab;

    void LateUpdate()
    {
        for (int i = 0; i < tabButtons.Count; i++)
        {
            tabPages[i].SetActive(tabButtons[i].isSelected);
            if (tabButtons[i].isSelected)
            {
                selectedTab = tabButtons[i];
            }
        }
    }

    public void OnTabSelected(MyButton button)
    {
        if (selectedTab != null)
        {
            selectedTab.SetSelected(false);
        }
        selectedTab = button;
        selectedTab.SetSelected(true);

        int index = tabButtons.IndexOf(button);
        for (int i = 0; i < tabPages.Count; i++)
        {
            tabPages[i].SetActive(i == index);
        }
    }
}
