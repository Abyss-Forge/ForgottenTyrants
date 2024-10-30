using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Unity.VisualScripting;
using ColorUtility = UnityEngine.ColorUtility;

[RequireComponent(typeof(Image))]
public class CrosshairController : Configurable<string, string>
{
    private Image _image;
    [SerializeField] private Crosshair[] _crosshairs;

    public CrosshairController() : base("CrosshairSettings", "Crosshair", "Target", "Color") { }

    protected override void InitializeDefaults()
    {
        _defaultSettings.Add(STags.Enemy, Color.red.ToHexString());
        _defaultSettings.Add(STags.Ally, Color.green.ToHexString());

        foreach (var item in _defaultSettings)
        {
            Debug.Log(item.Key + item.Value);
        }
    }

    protected override void OnAwake()
    {
        _image = GetComponent<Image>();
    }

    private void Start()
    {
        SetCrosshair(_crosshairs[0]);
    }

    void LateUpdate()
    {
        Ray rayOrigin = Camera.main.ScreenPointToRay(_image.transform.position);
        RaycastHit hitInfo;
        Color color = Color.white;

        if (Physics.Raycast(rayOrigin, out hitInfo))
        {
            if (hitInfo.collider != null)
            {
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
        return Color.blue;
    }

}
