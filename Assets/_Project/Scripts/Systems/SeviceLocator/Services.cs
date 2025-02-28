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




}