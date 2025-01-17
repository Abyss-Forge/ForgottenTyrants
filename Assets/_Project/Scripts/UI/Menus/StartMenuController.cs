using System.Collections;
using System.Collections.Generic;
using ForgottenTyrants;
using UnityEngine.SceneManagement;
using UnityEngine;
using UnityEngine.UI;
using Systems.EventBus;

public class StartMenuController : MonoBehaviour
{
    [SerializeField] private Button _playButton, _settingsButton, _quitButton;
    [SerializeField] private GameObject _settingsMenu;

    void OnEnable()
    {
        _playButton.onClick.AddListener(Play);
        _quitButton.onClick.AddListener(Quit);
        _settingsButton.onClick.AddListener(OpenSettings);
    }

    void OnDisable()
    {
        _playButton.onClick.RemoveAllListeners();
        _quitButton.onClick.RemoveAllListeners();
        _settingsButton.onClick.RemoveAllListeners();
    }

    private void Play()
    {
        EventBus<LoadSceneEvent>.Raise(new LoadSceneEvent { SceneGroupToLoad = 1 });
    }

    private void Quit()
    {
        Debug.Log("Exiting game...");
        Application.Quit();
    }

    private void OpenSettings()
    {
        _settingsMenu.SetActive(true);
    }

}