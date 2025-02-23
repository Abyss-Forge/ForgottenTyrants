using Unity.Multiplayer.Samples.Utilities;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PauseMenuController : MonoBehaviour
{
    [SerializeField] private Button _leaveButton;

    void OnEnable()
    {
        _leaveButton.onClick.AddListener(ExitLobby);
    }

    void OnDisable()
    {
        _leaveButton.onClick.RemoveAllListeners();
    }

    private void ExitLobby()
    {
        NetworkManager.Singleton.Shutdown();
        SceneLoaderWrapper.Instance.LoadScene(ForgottenTyrants.Scene.MultiplayerMenu, useNetworkSceneManager: false, LoadSceneMode.Single);
    }

}