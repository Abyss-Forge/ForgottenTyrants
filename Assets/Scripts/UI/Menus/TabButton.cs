using System.Collections;
using UnityEngine.EventSystems;
using UnityEngine;

public class TabButton : MyButton   //composicion
{
    public TabGroup tabGroup;

    public override void OnPointerClick(PointerEventData eventData)
    {
        base.OnPointerClick(eventData);
        tabGroup.OnTabSelected(this);
    }
}
