using UnityEngine;

namespace Systems.EventBus
{
    public interface IBusEvent { }

    public struct TestEvent : IBusEvent { }

    public struct PlayerDeathEvent : IBusEvent { }

    public struct PlayerRespawnEvent : IBusEvent { }

    public struct PlayerMovementEvent : IBusEvent
    {
        public bool Activate;
    }

    public struct LoadSceneEvent : IBusEvent
    {
        public int SceneGroupToLoad;
        public bool IsIndeterminate;
    }

    public struct AbilityStateChangedEvent : IBusEvent
    {
        public AbilityStateMachine ability;
    }

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