using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;


public class SpeedAreaScript : NetworkBehaviour
{
    [SerializeField] private float _speed, _durationSpeedBoost;
    private static Dictionary<ulong, Coroutine> _activeBoostsGlobal = new();

    private IEnumerator BoostSpeedServerCoroutine(PlayerController playerController)
    {
        // Ajustamos la velocidad en el servidor
        playerController.SetSpeed(_speed);

        // Si tu movimiento es client-driven, avisa al cliente
        SetSpeed_ClientRpc(0, _speed);

        // Esperamos la duración del boost
        yield return new WaitForSeconds(_durationSpeedBoost);

        // Quitamos el boost en el servidor
        playerController.SetSpeed(-_speed);

        // Y avisamos al cliente de nuevo
        SetSpeed_ClientRpc(0, -_speed);

        // Lo sacamos del diccionario de boosts activos
        _activeBoostsGlobal.Remove(0);
    }

    [Rpc(SendTo.ClientsAndHost)]
    private void SetSpeed_ClientRpc(ulong targetClientId, float speedDelta)
    {
        // Si este no es el cliente objetivo, no hacemos nada
        if (NetworkManager.Singleton.LocalClientId != targetClientId) return;

        PlayerController localPC = FindLocalPlayerController();
        if (localPC != null)
        {
            // Ajustamos su velocidad local
            localPC.SetSpeed(speedDelta);
        }
    }

    private PlayerController FindLocalPlayerController()
    {
        foreach (var kv in NetworkManager.Singleton.SpawnManager.SpawnedObjects)
        {
            var netObj = kv.Value;
            // Queremos buscar un objeto que sea nuestro (IsOwner) y que tenga PlayerController
            if (netObj != null && netObj.IsOwner && netObj.TryGetComponent(out PlayerController pc))
            {
                return pc;
            }
        }
        return null;
    }

    private void OnTriggerEnter(Collider other)
    {
        // Solo el servidor debe manejar la lógica de dar/eliminar boosts 
        if (!IsServer) return;

        // Verificamos si es un player
        if (!other.CompareTag("Player")) return;

        // Obtenemos el PlayerController
        PlayerController playerController = other.GetComponentInChildren<PlayerController>();
        if (!playerController)
        {
            playerController = other.GetComponentInChildren<PlayerController>();
        }
        if (!playerController) return;
        // Sacamos el clientId de ese objeto

        ulong clientId = 0;

        // Si ya tiene boost activo, no hacemos nada
        if (_activeBoostsGlobal.ContainsKey(clientId))
        {
            return;
        }
        // Iniciamos la corrutina que maneja el boost en el servidor
        Coroutine co = StartCoroutine(BoostSpeedServerCoroutine(playerController));
        _activeBoostsGlobal[clientId] = co;
    }
}
