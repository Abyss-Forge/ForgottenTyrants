using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Threading.Tasks;

public class LoadingScreenController : MonoBehaviour
{
    [SerializeField] private Image _progressBar;
    [SerializeField] private float _transitionDuration = 1;

    private float _target;
    private bool _isLoading;

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

}