using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Threading.Tasks;

public class ClientMenu : MonoBehaviour
{
    [SerializeField] private Button _closeButton, _joinLobbyButton;
    [SerializeField] private TMP_InputField _joinCodeInputField;
    [SerializeField] private TMP_Text _joinErrorText;

    private bool _isJoining;

    void OnEnable()
    {
        _closeButton.onClick.AddListener(Close);
        _joinLobbyButton.onClick.AddListener(StartClient);

        _joinCodeInputField.onValueChanged.AddListener(OnCodeInputFieldChanged);
        _joinLobbyButton.interactable = false;
        _joinErrorText.text = null;
    }

    void OnDisable()
    {
        _closeButton.onClick.RemoveAllListeners();
        _joinLobbyButton.onClick.RemoveAllListeners();

        _joinCodeInputField.onValueChanged.RemoveAllListeners();
    }

    private void OnCodeInputFieldChanged(string text)
    {
        bool isCodeValid = text.Length == 6;
        _joinLobbyButton.interactable = isCodeValid;
    }

    private async void StartClient()
    {
        if (_isJoining) return;
        _isJoining = true;

        try
        {
            await ClientManager.Instance.StartClient(_joinCodeInputField.text);
        }
        catch
        {
            _joinErrorText.text = $"Cannot find room {_joinCodeInputField.text.ToUpper()}";
            //TODO preguntar a oscar, porque esto no deberia crashear.
        }

        _isJoining = false;
    }

    private void Close()
    {
        gameObject.SetActive(false);
    }

}