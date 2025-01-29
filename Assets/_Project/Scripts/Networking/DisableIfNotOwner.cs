using System.Collections.Generic;
using System.Linq;
using Unity.Netcode;
using UnityEngine;

public class DisableIfNotOwner : NetworkBehaviour
{
    [Tooltip("Leave empty to directly disable the GameObject")]
    [SerializeField] private List<MonoBehaviour> _scriptsToDisable = new();

    void OnValidate()
    {
        // Remove duplicates from list
        _scriptsToDisable = _scriptsToDisable.Distinct().ToList();
    }

    public override void OnNetworkSpawn()
    {
        Debug.Log("OnNetworkSpawn ejecutado.");
        TryDisable();
    }

    void Awake()
    {
        Debug.Log("Awake ejecutado.");
    }

    private void TryDisable()
    {
        if (IsOwner) return;

        if (_scriptsToDisable.Count > 0)
        {
            foreach (var script in _scriptsToDisable)
            {
                script.enabled = false;
            }
        }
        else
        {
            gameObject.SetActive(false);
        }
    }

}