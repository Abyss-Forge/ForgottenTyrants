using System;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace Systems.SceneManagement
{
    public class SceneLoader : MonoBehaviour
    {
        [SerializeField] Image _loadingBarFill;
        [SerializeField] TMP_Text _percentajeText;
        [SerializeField] float _fillSpeed = 1f;
        [SerializeField] Canvas _loadingCanvas;
        [SerializeField] Camera _loadingCamera;
        [SerializeField] SceneGroup[] _sceneGroups;

        float _targetProgress;
        bool _isLoading;

        public readonly SceneGroupManager _manager = new();

        EventBinding<SceneEvent> _sceneEventBinding;

        void OnEnable()
        {
            _sceneEventBinding = new EventBinding<SceneEvent>(HandleSceneEvent);
            EventBus<SceneEvent>.Register(_sceneEventBinding);
        }

        void OnDisable()
        {
            EventBus<SceneEvent>.Deregister(_sceneEventBinding);
        }

        async void HandleSceneEvent(SceneEvent playerEvent)
        {
            if (_sceneGroups.IsInRange(playerEvent.SceneGroupToLoad)) await LoadSceneGroup(playerEvent.SceneGroupToLoad);
        }

        void Awake()
        {
            // TODO can remove
            _manager.OnSceneLoaded += sceneName => Debug.Log("Loaded: " + sceneName);
            _manager.OnSceneUnloaded += sceneName => Debug.Log("Unloaded: " + sceneName);
            _manager.OnSceneGroupLoaded += () => Debug.Log("Scene group loaded");
        }

        async void Start()
        {
            await LoadSceneGroup(0);
        }

        void Update()
        {
            if (!_isLoading) return;

            UpdateBarFill();
            UpdatePercentageText();
        }

        private void UpdateBarFill()
        {
            float currentFillAmount = _loadingBarFill.fillAmount;
            float progressDifference = Mathf.Abs(currentFillAmount - _targetProgress);

            float dynamicFillSpeed = progressDifference * _fillSpeed;

            _loadingBarFill.fillAmount = Mathf.Lerp(currentFillAmount, _targetProgress, Time.deltaTime * dynamicFillSpeed);
        }

        private void UpdatePercentageText()
        {
            _percentajeText.text = $"{_loadingBarFill.fillAmount * 100:F0}%";

            Vector3 position = _percentajeText.rectTransform.localPosition;
            RectTransform parentRect = _percentajeText.rectTransform.parent as RectTransform;

            float maxPosX = (parentRect.sizeDelta.x - _percentajeText.rectTransform.sizeDelta.x) / 2;
            position.x = Mathf.Lerp(-maxPosX, maxPosX, _loadingBarFill.fillAmount / 2);

            _percentajeText.rectTransform.localPosition = position;
        }

        public async Task LoadSceneGroup(string name)
        {
            for (int i = 0; i < _sceneGroups.Length; i++)
            {
                if (_sceneGroups[i].GroupName == name)
                {
                    await LoadSceneGroup(i);
                    return;
                }
            }
            throw new Exception($"Scene group with name {name} does not exist");
        }

        public async Task LoadSceneGroup(int index)
        {
            _loadingBarFill.fillAmount = 0f;
            _targetProgress = 1f;

            if (index < 0 || index >= _sceneGroups.Length)
            {
                Debug.LogError("Invalid scene group index: " + index);
                return;
            }

            LoadingProgress progress = new LoadingProgress();
            progress.Progressed += target => _targetProgress = Mathf.Max(target, _targetProgress);

            EnableLoadingCanvas();
            await _manager.LoadScenes(_sceneGroups[index], progress);
            EnableLoadingCanvas(false);
        }

        void EnableLoadingCanvas(bool enable = true)
        {
            _isLoading = enable;
            _loadingCanvas.gameObject.SetActive(enable);
            _loadingCamera.gameObject.SetActive(enable);
        }

    }

    public class LoadingProgress : IProgress<float>
    {
        public event Action<float> Progressed;

        const float ratio = 1f;

        public void Report(float value)
        {
            Progressed?.Invoke(value / ratio);
        }
    }

}