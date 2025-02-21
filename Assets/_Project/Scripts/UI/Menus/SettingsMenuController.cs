using Systems.GameManagers;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

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
        if (context.performed) _settingsMenu.gameObject.SetActive(!_settingsMenu.gameObject.activeSelf);
    }

    private void Close()
    {
        Debug.Log("sfsdf");
        _settingsMenu.gameObject.SetActive(false);
    }

}