using System.Collections;
using UnityEngine.EventSystems;
using UnityEngine;

public class TabButton : MyButton   //composicion
{
    public TabGroup _tabGroup;

    public override void OnPointerClick(PointerEventData eventData)
    {
        base.OnPointerClick(eventData);
        _tabGroup.OnTabSelected(this);
    }
}
