using Systems.AudioManagement;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class AudioSettingsMenuController : MonoBehaviour
{
    [SerializeField] private RectTransform _elementHolder;
    [SerializeField] private AudioVolumeElement _elementPrefab;

    private bool _isUpdating = false;

    void Awake()
    {
        Initialize();
    }

    private void Initialize()
    {
        foreach (var key in AudioManager.Instance.Helper.VolumeKeyList)
        {
            AudioVolumeElement instance = Instantiate(_elementPrefab, _elementHolder);

            TMP_Text text = instance.Text;
            Toggle toggle = instance.Toggle;
            Slider slider = instance.Slider;

            text.text = key;
            float volume = AudioManager.Instance.Helper.GetVolume(key);
            toggle.isOn = volume > 0;
            slider.value = volume;

            toggle.onValueChanged.AddListener((value) => HandleToggle(value, key, slider));
            slider.onValueChanged.AddListener((value) => HandleSlider(value, key, toggle));
        }
    }

    private void HandleToggle(bool value, string key, Slider slider)
    {
        if (_isUpdating) return;
        _isUpdating = true;

        AudioManager.Instance.Helper.ToggleVolume(key);
        slider.value = AudioManager.Instance.Helper.GetVolume(key);

        _isUpdating = false;
    }

    private void HandleSlider(float value, string key, Toggle toggle)
    {
        if (_isUpdating) return;
        _isUpdating = true;

        AudioManager.Instance.Helper.SetVolume(key, value);
        toggle.isOn = value > 0;

        _isUpdating = false;
    }

}