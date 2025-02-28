using Systems.ServiceLocator;
using UnityEngine;

public class MockServiceUsageExample : MonoBehaviour
{
    MockSerializer _serializer;

    void Awake()
    {
        ServiceLocator.Global.Register<ISerializer>(_serializer = new MockSerializer());
        ServiceLocator.ForSceneOf(this).Register<ISerializer>(_serializer = new MockSerializer());
        ServiceLocator.For(this).Register<ISerializer>(_serializer = new MockSerializer());
    }

    void Start()
    {
        ServiceLocator.For(this).Get(out _serializer);

        _serializer.Test();
    }

}