using System;
using Unity.Services.Authentication;
using Unity.Services.Core;
using UnityEngine;
using UnityEngine.UI;

public class MultiplayerMenuController : MonoBehaviour
{
    [SerializeField] private GameObject _connectingPanel, _menuPanel, _hostPanel, _clientPanel, _lobbiesPanel;
    [SerializeField] private Button _hostButton, _clientButton, _lobbiesButton;

    void OnEnable()
    {
        _hostButton.onClick.AddListener(() => _hostPanel.SetActive(true));
        _clientButton.onClick.AddListener(() => _clientPanel.SetActive(true));
        _lobbiesButton.onClick.AddListener(() => _lobbiesPanel.SetActive(true));
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

}