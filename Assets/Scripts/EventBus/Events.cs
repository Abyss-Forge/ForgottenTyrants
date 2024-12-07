using UnityEngine;

public interface IEvent { }

public struct TestEvent : IEvent { }

public struct PlayerDeathEvent : IEvent { }

public struct SceneEvent : IEvent
{
    public int SceneGroupToLoad;
}

/*
public class ExampleWithTestEvent : MonoBehaviour
{
    EventBinding<TestEvent> testEventBinding;

    void OnEnable()
    {
        testEventBinding = new EventBinding<TestEvent>(HandleTestEvent);
        EventBus<TestEvent>.Register(testEventBinding);
    }

    void OnDisable()
    {
        EventBus<TestEvent>.Deregister(testEventBinding);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.P)) EventBus<TestEvent>.Raise(new TestEvent());
    }

    private void HandleTestEvent()
    {
        Debug.Log("Test event");
    }
}
*/