using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class AudioVolumeElement : MonoBehaviour
{
    [SerializeField] private TMP_Text _text;
    [SerializeField] private Toggle _toggle;
    [SerializeField] private Slider _slider;

    public TMP_Text Text => _text;
    public Toggle Toggle => _toggle;
    public Slider Slider => _slider;
}