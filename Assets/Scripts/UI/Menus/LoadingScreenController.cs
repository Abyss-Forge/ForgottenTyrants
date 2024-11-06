using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Threading.Tasks;
using System;
using System.IO;
using System.Linq;
using TMPro;

[RequireComponent(typeof(Image))]
public class LoadingScreenController : MonoBehaviour
{
    private Image _image;
    [SerializeField] private TMP_Text _messageText;
    [SerializeField] private Image _progressBar;
    [SerializeField] private float _transitionDuration = 1;

    private string _imagesFolderPath = SGlobalSettings.LoadingScreenBackgroundsFolder;
    private string _textFolderPath = SGlobalSettings.LoadingScreenMessagesFolder;
    private float _target;
    private bool _isLoading;

    void Awake()
    {
        _image = GetComponent<Image>();
    }

    void OnEnable()
    {
        SetRandomBackground();
        SetRandomMessage();
    }

    void Update()
    {
        if (_isLoading)
        {
            _progressBar.fillAmount = Mathf.MoveTowards(_progressBar.fillAmount, _target, 3 * Time.deltaTime);
        }
    }

    public async void Load(AsyncOperation operation)
    {
        _target = 0;
        _progressBar.fillAmount = 0;
        operation.allowSceneActivation = false;

        //StartCoroutine(MyAudioManager.Instance.FadeOutAudio(transitionDuration));
        //StartCoroutine(MyScreenManager.Instance.FadeOutScreen(transitionDuration));
        await Sleep();
        //StartCoroutine(MyScreenManager.Instance.FadeInScreen(transitionDuration));

        await Sleep();
        _isLoading = true;
        do
        {
            await Task.Delay(100);
            _target = operation.progress;
        }
        while (operation.progress < 0.9f);
        await Sleep();

        //StartCoroutine(MyScreenManager.Instance.FadeOutScreen(transitionDuration));
        await Sleep();
        operation.allowSceneActivation = true;
        await Task.Delay(100);
        //StartCoroutine(MyScreenManager.Instance.FadeInScreen(transitionDuration));
        //StartCoroutine(MyAudioManager.Instance.FadeInAudio(transitionDuration));
        gameObject.SetActive(false);
    }

    private async Task Sleep()
    {
        await Task.Delay((int)(_transitionDuration * 1000));
    }

    private void SetRandomMessage()
    {
        if (!File.Exists(_textFolderPath))
        {
            Debug.LogWarning("File not found: " + _textFolderPath);
            return;
        }

        string[] messages = File.ReadAllLines(_textFolderPath);

        if (messages.Length == 0)
        {
            Debug.LogWarning("No message found in file: " + _textFolderPath);
            return;
        }

        string randomMessage = messages[UnityEngine.Random.Range(0, messages.Length)];
        _messageText.text = randomMessage;
    }

    private void SetRandomBackground()
    {
        string folderPath = _imagesFolderPath;
        string[] files = Directory.GetFiles(folderPath, "*.*")
                             .Where(file => file.EndsWith(".jpg", StringComparison.OrdinalIgnoreCase) ||
                                            file.EndsWith(".jpeg", StringComparison.OrdinalIgnoreCase) ||
                                            file.EndsWith(".png", StringComparison.OrdinalIgnoreCase))
                             .ToArray();


        if (files.Length == 0)
        {
            Debug.LogWarning("No files found on folder: " + folderPath + files.Length);
            return;
        }

        string randomFile = files[UnityEngine.Random.Range(0, files.Length)];

        byte[] fileData = File.ReadAllBytes(randomFile);
        Texture2D texture = new Texture2D(2, 2);
        texture.LoadImage(fileData);

        Sprite sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.5f, 0.5f));

        _image.sprite = sprite;
    }

}