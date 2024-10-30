using System.Collections;
using UnityEngine.EventSystems;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Image))]
public class MyButton : MonoBehaviour, IPointerEnterHandler, IPointerClickHandler, IPointerExitHandler
{
    private Image _image;

    [Header("Config")]
    [SerializeField] private Sprite _idleSprite, _hoverSprite, _activeSprite;
    [SerializeField] private bool _isToggleable, _isDependent, _isActiveByDefault;

    private bool _isSelected;
    public bool IsSelected => _isSelected;

    private enum _PointerAction
    {
        Enter, Click, Exit
    }

    void Start()
    {
        _image = GetComponent<Image>();
        if (_image == null)
        {
            gameObject.AddComponent<Image>();
        }

        _isToggleable = _isDependent || _isToggleable;
        SetSelected(_isActiveByDefault);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (CanBeModified(_PointerAction.Enter))
        {
            _image.sprite = _hoverSprite;
        }
    }

    public virtual void OnPointerClick(PointerEventData eventData)
    {
        if (CanBeModified(_PointerAction.Click))
        {
            SetSelected(!_isSelected); //toggle
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (CanBeModified(_PointerAction.Exit))
        {
            _image.sprite = _idleSprite;
        }
    }

    public void SetSelected(bool selected)
    {
        _isSelected = selected;
        _image.sprite = selected ? _activeSprite : _idleSprite;
    }

    private bool CanBeModified(_PointerAction action)
    {
        if (!_isSelected)
        {
            return true;
        }

        if (_isDependent)
        {
            return false;
        }

        if (_isToggleable)
        {
            return action == _PointerAction.Click;
        }

        return true;
    }

}
