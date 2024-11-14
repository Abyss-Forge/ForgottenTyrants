using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Unity.VisualScripting;
using ColorUtility = UnityEngine.ColorUtility;
using ForgottenTyrants;

[RequireComponent(typeof(Image))]
public class CrosshairController : Configurable<string, string>
{
    private Image _image;

    [SerializeField] private Crosshair[] _crosshairs;
    [SerializeField] private Color _defaultColor;

    private GameObject _targetObject;
    public GameObject TargetObject => _targetObject;

    public CrosshairController() : base("CrosshairSettings", "Crosshair", "Target", "Color") { }

    protected override void InitializeDefaults()
    {
        _defaultSettings.Add(Tag.Enemy, Color.red.ToHexString());
        _defaultSettings.Add(Tag.Ally, Color.green.ToHexString());
    }

    protected override void OnAwake()
    {
        _image = GetComponent<Image>();

        SetCrosshair(_crosshairs[0]);
    }

    void LateUpdate()
    {
        PerformRaycast();
    }

    private void PerformRaycast()
    {
        Ray rayOrigin = Camera.main.ScreenPointToRay(_image.transform.position);
        Color color = _defaultColor;

        if (Physics.Raycast(rayOrigin, out RaycastHit hitInfo))
        {
            if (hitInfo.collider != null)
            {
                _targetObject = hitInfo.collider.gameObject;

                string tag = hitInfo.collider.gameObject.tag;
                foreach (var item in _settings)
                {
                    if (tag == item.Key)
                    {
                        color = StringToColor(item.Value);
                    }
                }
            }
        }

        _image.color = color;
    }

    public void SetCrosshair(Crosshair crosshair)
    {
        _image.sprite = crosshair.Sprite;
        int size = crosshair.Size;
        _image.rectTransform.sizeDelta = new Vector2(size, size);
    }

    private Color StringToColor(string colorStr)
    {
        if (ColorUtility.TryParseHtmlString("#" + colorStr, out Color colorObj))
        {
            return colorObj;
        }
        return _defaultColor;
    }

}
