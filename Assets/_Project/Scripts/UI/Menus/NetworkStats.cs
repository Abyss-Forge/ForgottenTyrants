using TMPro;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using UnityEngine;

public class NetworkStats : MonoBehaviour
{
    [Header("Ping Display")]
    [SerializeField] private TextMeshProUGUI _pingText;
    [SerializeField] private float _updateRate = 1f;

    float _nextUpdateTime, _ping;

    void Update()
    {
        if (Time.unscaledTime >= _nextUpdateTime)
        {
            _nextUpdateTime = Time.unscaledTime + _updateRate;

            ShowPing();
        }
    }

    private void ShowPing()
    {
        if (NetworkManager.Singleton.IsConnectedClient)
        {
            var transport = NetworkManager.Singleton.NetworkConfig.NetworkTransport;    //  NetworkTransport / UnityTransport
            if (transport != null)
            {
                _ping = transport.GetCurrentRtt(NetworkManager.Singleton.LocalClientId);

                _pingText.text = $"Ping: {Mathf.RoundToInt(_ping)} ms";
            }
            else
            {
                _pingText.text = "Ping: N/A";
            }
        }
        else
        {
            _pingText.text = "Disconnected";
        }
    }

}