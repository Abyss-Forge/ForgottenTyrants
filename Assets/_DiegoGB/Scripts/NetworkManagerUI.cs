using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Unity.Netcode;

//Clase de testing inicial para probar el multijugador mas básico
public class NetworkManagerUI : MonoBehaviour
{
    [SerializeField] Button _serverButton;
    [SerializeField] Button _hostButton;
    [SerializeField] Button _clientButton;

    private void Awake()
    {
        _serverButton.onClick.AddListener(() =>
        {
            NetworkManager.Singleton.StartServer();
        });
        _hostButton.onClick.AddListener(() =>
        {
            NetworkManager.Singleton.StartHost();
        });
        _clientButton.onClick.AddListener(() =>
        {
            NetworkManager.Singleton.StartClient();
        });
    }
}
