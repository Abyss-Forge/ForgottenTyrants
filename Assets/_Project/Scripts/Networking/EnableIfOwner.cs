using System.Collections.Generic;
using System.Linq;
using Unity.Netcode;
using UnityEngine;

public class EnableIfOwner : NetworkBehaviour
{
    //TODO unique attribute tag to unallow duplicates
    [SerializeField] private List<BeginDisabled> _gameObjectsToEnable = new();
    [SerializeField] private List<Component> _componentsToEnable = new();

    void OnValidate()
    {
        // Remove duplicates from list
        _gameObjectsToEnable = _gameObjectsToEnable.Distinct().ToList();
        _componentsToEnable = _componentsToEnable.Distinct().ToList();
    }

    public override void OnNetworkSpawn()
    {
        if (!IsOwner) return;

        TryEnableComponents();
        TryEnableGameObjects();
    }

    private void TryEnableGameObjects()
    {
        foreach (var script in _gameObjectsToEnable)
        {
            script.gameObject.SetActive(true);
        }
    }

    private void TryEnableComponents()
    {
        foreach (Component component in _componentsToEnable)
        {
            if (component is Behaviour behaviour)
            {
                behaviour.enabled = true;
            }
            else if (component is Collider collider)
            {
                collider.enabled = true;
            }
            else if (component is Renderer renderer)
            {
                renderer.enabled = true;
            }
        }
    }

}