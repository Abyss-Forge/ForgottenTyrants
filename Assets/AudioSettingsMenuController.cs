using System.Runtime.InteropServices;
using Systems.AudioManagement;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class AudioSettingsMenuController : MonoBehaviour
{
    [SerializeField] private RectTransform _elementHolder;
    [SerializeField] private AudioVolumeElement _elementPrefab;

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
            Slider slider = instance.Slider;
            Toggle toggle = instance.Toggle;

            text.text = key;
            float volume = PlayerPrefs.GetFloat(key, 0.5f);
            slider.value = volume;
            toggle.isOn = volume > 0;

            toggle.onValueChanged.AddListener((value) => HandleToggle(value, key, slider));
            slider.onValueChanged.AddListener((value) => HandleSlider(value, key, toggle));
        }
    }

    private void HandleToggle(bool value, string key, Slider slider)
    {
        AudioManager.Instance.Helper.ToggleVolume(key);
        slider.value = AudioManager.Instance.Helper.GetVolume(key);
    }

    private void HandleSlider(float value, string key, Toggle toggle)
    {
        AudioManager.Instance.Helper.SetVolume(key, value);
        toggle.isOn = AudioManager.Instance.Helper.GetVolume(key) > 0;
    }

}