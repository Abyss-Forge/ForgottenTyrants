using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StartMenuController : MonoBehaviour
{
    [Header("Buttons")]
    [SerializeField] private Button playButton;
    [SerializeField] private Button quitButton;
    [SerializeField] private Button settingsButton;
    [SerializeField] private GameObject settingsMenu;

    void Start()
    {
        playButton.onClick.AddListener(Play);
        quitButton.onClick.AddListener(Quit);
        settingsButton.onClick.AddListener(OpenSettings);
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
