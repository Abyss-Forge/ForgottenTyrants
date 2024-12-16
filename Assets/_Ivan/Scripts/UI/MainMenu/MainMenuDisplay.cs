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
    [SerializeField] private Button _hostButton, _clientButton, _lobbiesButton;
    [SerializeField] private TMP_InputField _joinCodeInputField;

    void OnEnable()
    {
        _hostButton.onClick.AddListener(StartHost);
        _clientButton.onClick.AddListener(StartClient);
        _lobbiesButton.onClick.AddListener(ShowLobbies);
    }

    void OnDisable()
    {
        _hostButton.onClick.RemoveAllListeners();
        _clientButton.onClick.RemoveAllListeners();
        _lobbiesButton.onClick.RemoveAllListeners();
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
        HostManager.Instance.StartHost();
    }

    public async void StartClient()
    {
        await ClientManager.Instance.StartClient(_joinCodeInputField.text);
    }

    private void ShowLobbies()
    {
        _lobbiesPanel.SetActive(true);
    }

}