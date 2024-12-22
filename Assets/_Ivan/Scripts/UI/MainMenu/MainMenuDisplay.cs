using System;
using TMPro;
using Unity.Netcode;
using Unity.Services.Authentication;
using Unity.Services.Core;
using Unity.Services.Relay;
using Unity.Services.Relay.Models;
using UnityEngine;
using UnityEngine.UI;

public class MainMenuDisplay : MonoBehaviour
{
    [SerializeField] private GameObject _connectingPanel, _menuPanel, _lobbiesPanel;
    [SerializeField] private Button _hostButton, _joinButton, _lobbiesButton;
    [SerializeField] private TMP_InputField _joinCodeInputField;
    [SerializeField] private TMP_Text _joinErrorText;
    [SerializeField] private Toggle _publicHostToggle;

    void OnEnable()
    {
        _hostButton.onClick.AddListener(StartHost);
        _joinButton.onClick.AddListener(StartClient);
        _lobbiesButton.onClick.AddListener(ShowLobbies);

        _joinCodeInputField.onValueChanged.AddListener(OnCodeInputFieldChanged);
        _joinButton.interactable = false;
        _joinErrorText.text = null;
    }

    void OnDisable()
    {
        _hostButton.onClick.RemoveAllListeners();
        _joinButton.onClick.RemoveAllListeners();
        _lobbiesButton.onClick.RemoveAllListeners();
    }

    void OnCodeInputFieldChanged(string text)
    {
        bool isCodeValid = text.Length == 6;
        _joinButton.interactable = isCodeValid;
    }

    async void Start()
    {
        try
        {
            await UnityServices.InitializeAsync();
            await AuthenticationService.Instance.SignInAnonymouslyAsync();
            Debug.Log($"Player Id: {AuthenticationService.Instance.PlayerId}");
        }
        catch (Exception e)
        {
            Debug.LogError(e);
            return;
        }

        _connectingPanel.SetActive(false);
        _menuPanel.SetActive(true);
    }

    public void StartHost()
    {
        HostManager.Instance.StartHost(!_publicHostToggle.isOn);
    }

    public async void StartClient()
    {
        try
        {
            await ClientManager.Instance.StartClient(_joinCodeInputField.text);
        }
        catch
        {
            _joinErrorText.text = $"Cannot find room {_joinCodeInputField.text.ToUpper()}";
            //TODO preguntar a oscar, porque esto no deberia crashear.
        }
    }

    private void ShowLobbies()
    {
        _lobbiesPanel.SetActive(true);
    }

}