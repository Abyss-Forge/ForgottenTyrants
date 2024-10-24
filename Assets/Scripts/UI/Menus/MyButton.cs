using System.Collections;
using UnityEngine.EventSystems;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Image))]
public class MyButton : MonoBehaviour, IPointerEnterHandler, IPointerClickHandler, IPointerExitHandler
{
    [Header("Config")]
    [SerializeField] private Sprite idleSprite;
    [SerializeField] private Sprite hoverSprite;
    [SerializeField] private Sprite activeSprite;
    [SerializeField] private bool isToggleable = false;
    [SerializeField] private bool isDependent = false;
    [SerializeField] private bool isActiveByDefault = false;

    public bool isSelected { get; private set; }
    private Image backgroundImage;

    private enum PointerAction
    {
        Enter, Click, Exit
    }

    void Start()
    {
        backgroundImage = GetComponent<Image>();
        if (backgroundImage == null)
        {
            gameObject.AddComponent<Image>();
        }

        isToggleable = isDependent || isToggleable;
        SetSelected(isActiveByDefault);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (CanBeModified(PointerAction.Enter))
        {
            backgroundImage.sprite = hoverSprite;
        }
    }

    public virtual void OnPointerClick(PointerEventData eventData)
    {
        if (CanBeModified(PointerAction.Click))
        {
            SetSelected(!isSelected); //toggle
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (CanBeModified(PointerAction.Exit))
        {
            backgroundImage.sprite = idleSprite;
        }
    }

    public void SetSelected(bool selected)
    {
        isSelected = selected;
        backgroundImage.sprite = selected ? activeSprite : idleSprite;
    }

    private bool CanBeModified(PointerAction action)
    {
        if (!isSelected)
        {
            return true;
        }

        if (isDependent)
        {
            return false;
        }

        if (isToggleable)
        {
            return action == PointerAction.Click;
        }

        return true;
    }

}
