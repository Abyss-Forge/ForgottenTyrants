using System.Collections.Generic;
using System.Linq;
using Unity.Netcode;
using UnityEngine;

public class DisableIfNotOwner : NetworkBehaviour
{
    [SerializeField] private List<MonoBehaviour> _scriptsToDisable = new();

    void OnValidate()
    {
        // Eliminar duplicados de la lista
        _scriptsToDisable = _scriptsToDisable.Distinct().ToList();
    }

    public override void OnNetworkSpawn()
    {
        TryDisable();
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