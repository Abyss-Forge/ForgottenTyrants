using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

[RequireComponent(typeof(Image))]
public class MyButton : MonoBehaviour, IPointerEnterHandler, IPointerClickHandler, IPointerExitHandler
{
    Image _image;

    [Header("Config")]
    [SerializeField] private Sprite _idleSprite, _hoverSprite, _activeSprite;
    [SerializeField] private bool _isToggleable, _isDependent, _isActiveByDefault;

    private bool _isSelected;
    public bool IsSelected => _isSelected;

    private enum EPointerAction
    {
        ENTER, CLICK, EXIT
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
        if (CanBeModified(EPointerAction.ENTER))
        {
            _image.sprite = _hoverSprite;
        }
    }

    public virtual void OnPointerClick(PointerEventData eventData)
    {
        if (CanBeModified(EPointerAction.CLICK))
        {
            SetSelected(!_isSelected); //toggle
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (CanBeModified(EPointerAction.EXIT))
        {
            _image.sprite = _idleSprite;
        }
    }

    public void SetSelected(bool selected)
    {
        _isSelected = selected;
        _image.sprite = selected ? _activeSprite : _idleSprite;
    }

    private bool CanBeModified(EPointerAction action)
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
            return action == EPointerAction.CLICK;
        }

        return true;
    }

}
