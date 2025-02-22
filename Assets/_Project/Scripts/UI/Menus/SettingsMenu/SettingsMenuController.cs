using Systems.GameManagers;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using Utils;

public class SettingsMenuController : MonoBehaviour
{
    [SerializeField] private Button _closeButton;
    [SerializeField] private RectTransform _settingsMenu;

    void OnEnable()
    {
        _closeButton.onClick.AddListener(Close);
        MyInputManager.Instance.Subscribe(EInputAction.PAUSE, OnPause);
    }

    void OnDisable()
    {
        _closeButton.onClick.RemoveAllListeners();
        MyInputManager.Instance.Unsubscribe(EInputAction.PAUSE, OnPause);
    }

    private void OnPause(InputAction.CallbackContext context)
    {
        if (context.performed) Close();
    }

    private void Close()
    {
        bool shouldEnableMenu = !_settingsMenu.gameObject.activeSelf;
        _settingsMenu.gameObject.SetActive(shouldEnableMenu);
        CursorHelper.Capture(!shouldEnableMenu);
    }

}