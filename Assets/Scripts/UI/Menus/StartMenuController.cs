using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StartMenuController : MonoBehaviour
{
    [SerializeField] private Button _playButton, _settingsButton, _quitButton;
    [SerializeField] private GameObject settingsMenu;

    void Start()
    {
        _playButton.onClick.AddListener(Play);
        _quitButton.onClick.AddListener(Quit);
        _settingsButton.onClick.AddListener(OpenSettings);
    }

    private void Play()
    {
        MySceneManager.Instance.LoadSceneWithLoadingScreen(1);
    }

    private void Quit()
    {
        Debug.Log("Exiting game...");
        Application.Quit();
    }

    private void OpenSettings()
    {
        settingsMenu.SetActive(true);
    }

}
