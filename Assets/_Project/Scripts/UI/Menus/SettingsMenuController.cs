using Systems.EventBus;
using UnityEngine;
using UnityEngine.UI;

public class SettingsMenuController : MonoBehaviour
{
    [SerializeField] private Button _closeButton;
    [SerializeField] private RectTransform _settingsMenu;

    void OnEnable()
    {
        _closeButton.onClick.AddListener(Close);
    }

    void OnDisable()
    {
        _closeButton.onClick.RemoveAllListeners();
    }

    private void Open()
    {
        _settingsMenu.gameObject.SetActive(true);
    }

    private void Close()
    {
        _settingsMenu.gameObject.SetActive(false);
    }


}