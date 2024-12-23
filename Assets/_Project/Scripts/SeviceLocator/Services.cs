using UnityEngine;

namespace Systems.ServiceLocator
{

    public interface ISerializer { }

    public class MockSerializer : ISerializer
    {
        public void Test()
        {
            Debug.Log("Serializer");
        }
    }

    public interface IAudioService { }

    public class MockAudioService : IAudioService
    {
        public void Test()
        {
            Debug.Log("Audio service");
        }
    }

    public interface IInputService { }

    public class MockInputService : IInputService
    {
        public void Test()
        {
            Debug.Log("Input service");
        }
    }

    public interface IGameService { }

    public class MockGameService : IGameService
    {
        public void Test()
        {
            Debug.Log("Input service");
        }
    }


}