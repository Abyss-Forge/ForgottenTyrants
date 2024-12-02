using System;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

namespace Systems.SceneManagement
{
    public class SceneLoader : MonoBehaviour
    {
        [SerializeField] Image _loadingBar;
        [SerializeField] float _fillSpeed = 0.5f;
        [SerializeField] Canvas _loadingCanvas;
        [SerializeField] Camera _loadingCamera;
        [SerializeField] SceneGroup[] _sceneGroups;

        float _targetProgress;
        bool _isLoading;

        EventBinding<SceneEvent> _sceneEventBinding;

        public readonly SceneGroupManager _manager = new();

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

        void Update()
        {
            if (!_isLoading) return;

            float currentFillAmount = _loadingBar.fillAmount;
            float progressDifference = Mathf.Abs(currentFillAmount - _targetProgress);

            float dynamicFillSpeed = progressDifference * _fillSpeed;

            _loadingBar.fillAmount = Mathf.Lerp(currentFillAmount, _targetProgress, Time.deltaTime * dynamicFillSpeed);
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
            _loadingBar.fillAmount = 0f;
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