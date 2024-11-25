using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Threading.Tasks;

public class ConfirmationDialogController : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _messageText;
    [SerializeField] private Button _confirmButton, _cancelButton;
    [SerializeField] private TextMeshProUGUI _confirmText, _cancelText;

    public async Task<bool> LaunchPopup(string message, string confirm = "confirm", string cancel = "cancel")
    {
        TaskCompletionSource<bool> tcs = new();

        _messageText.text = message;
        _confirmText.text = confirm;
        _cancelText.text = cancel;
        _confirmButton.onClick.AddListener(() => tcs.SetResult(true));
        _cancelButton.onClick.AddListener(() => tcs.SetResult(false));

        gameObject.SetActive(true);

        bool result = await tcs.Task;

        gameObject.SetActive(false);
        _confirmButton.onClick.RemoveAllListeners();
        _cancelButton.onClick.RemoveAllListeners();

        return result;
    }

}