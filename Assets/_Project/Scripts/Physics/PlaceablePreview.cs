using UnityEngine;

[RequireComponent(typeof(Renderer), typeof(MeshFilter))]
public class PlaceablePreview : MonoBehaviour
{
    Renderer _renderer;

    [SerializeField] private Color _validColor, _invalidColor;

    private bool _isValid;
    public bool IsValid => _isValid;

    void Awake()
    {
        _renderer = GetComponent<Renderer>();
    }

    public void ToggleValid() => SetValid(!_isValid);
    public void SetInvalid() => SetValid(false);
    public void SetValid(bool valid = true)
    {
        _isValid = valid;

        UpdateColor();
    }

    private void UpdateColor()
    {
        Material material = _renderer.material;
        Color newColor = _isValid ? _validColor : _invalidColor;
        material.color = newColor;
    }
}