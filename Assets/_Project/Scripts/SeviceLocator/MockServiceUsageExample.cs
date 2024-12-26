using Systems.ServiceLocator;
using UnityEngine;

public class MockServiceUsageExample : MonoBehaviour
{
    MockSerializer _serializer;
    MockAudioService _audioService;
    MockInputService _inputService;
    MockGameService _gameService;

    void Awake()
    {
        ServiceLocator.Global.Register<ISerializer>(_serializer = new MockSerializer());
        ServiceLocator.ForSceneOf(this).Register<IAudioService>(_audioService = new MockAudioService());
        ServiceLocator.For(this).Register<IInputService>(_inputService = new MockInputService());
    }

    void Start()
    {
        ServiceLocator.For(this)
            .Get(out _serializer)
            .Get(out _audioService)
            .Get(out _inputService)
            .Get(out _gameService);

        _serializer.Test();
        _audioService.Test();
        _inputService.Test();
        _gameService.Test();
    }

}