using System;
using System.Linq;
using Unity.Netcode;
using UnityEngine;

public class EnableIfOwner : NetworkBehaviour
{
    //TODO unique attribute tag to unallow duplicates
    [SerializeField,] private BeginDisabled[] _gameObjectsToEnable;
    [SerializeField,] private Component[] _componentsToEnable;

    void OnValidate()
    {
        // Remove duplicates from list
        try
        {
            _gameObjectsToEnable = _gameObjectsToEnable.ToList().Distinct().ToArray();
            _componentsToEnable = _componentsToEnable.ToList().Distinct().ToArray();
        }
        catch (ArgumentNullException) { }
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
            if (component is Behaviour behaviour) behaviour.enabled = true;
            else if (component is Collider collider) collider.enabled = true;
            else if (component is Renderer renderer) renderer.enabled = true;
        }
    }

}