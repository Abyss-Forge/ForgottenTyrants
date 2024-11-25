using System.Collections;
using System.Collections.Generic;
using ForgottenTyrants;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class StartMenuController : MonoBehaviour
{
    [SerializeField] private Button _playButton, _settingsButton, _quitButton;
    [SerializeField] private GameObject _settingsMenu;

    void Start()
    {
        _playButton.onClick.AddListener(Play);
        _quitButton.onClick.AddListener(Quit);
        _settingsButton.onClick.AddListener(OpenSettings);
    }

    private void Play()
    {
        // MySceneManager.Instance.LoadSceneWithLoadingScreen(Scene.Next);
        SceneManager.LoadScene(ForgottenTyrants.Scene.Next);
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
