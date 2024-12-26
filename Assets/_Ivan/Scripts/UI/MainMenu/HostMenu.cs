using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class HostMenu : MonoBehaviour
{
    [SerializeField] private Button _closeButton, _createLobbyButton;
    [SerializeField] private Toggle _publicLobbyToggle;
    [SerializeField] private TMP_InputField _lobbyNameInputField;

    private bool _isJoining;

    void OnEnable()
    {
        _closeButton.onClick.AddListener(Close);
        _createLobbyButton.onClick.AddListener(StartHost);
        _publicLobbyToggle.onValueChanged.AddListener(OnPublicToggleChanged);
        _lobbyNameInputField.onValueChanged.AddListener(OnNameInputFieldChanged);

        _createLobbyButton.interactable = CanHost();
    }

    void OnDisable()
    {
        _closeButton.onClick.RemoveAllListeners();
        _createLobbyButton.onClick.RemoveAllListeners();
        _publicLobbyToggle.onValueChanged.RemoveAllListeners();
        _lobbyNameInputField.onValueChanged.RemoveAllListeners();
    }

    private void OnPublicToggleChanged(bool isOn)
    {
        _lobbyNameInputField.interactable = isOn;

        _createLobbyButton.interactable = CanHost();
    }

    private void OnNameInputFieldChanged(string text)
    {
        _createLobbyButton.interactable = CanHost();
    }

    private bool CanHost()
    {
        if (!_publicLobbyToggle.isOn) return true;

        string lobbyName = _lobbyNameInputField.text;
        bool isNameValid = (lobbyName.Length >= 4) && (lobbyName.Length <= 12);

        return isNameValid;
    }

    private async void StartHost()
    {
        if (_isJoining) return;
        _isJoining = true;

        bool isPublic = _publicLobbyToggle.isOn;
        string name = isPublic ? _lobbyNameInputField.text : null;
        await HostManager.Instance.StartHost(!isPublic, name);

        _isJoining = false;
    }

    private void Close()
    {
        gameObject.SetActive(false);
    }

}