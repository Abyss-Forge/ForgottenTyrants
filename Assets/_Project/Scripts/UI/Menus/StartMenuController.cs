using Systems.EventBus;
using UnityEngine;
using UnityEngine.UI;

public class StartMenuController : MonoBehaviour
{
    [SerializeField] private Button _playButton, _settingsButton, _quitButton;
    [SerializeField] private RectTransform _settingsMenu;

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
        _settingsMenu.gameObject.SetActive(true);
    }

}