using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using ForgottenTyrants;
using Unity.Netcode;
using UnityEngine;
using Vector3 = UnityEngine.Vector3;

public class SpeedAreaScript : NetworkBehaviour
{
    [SerializeField] private float _speed;
    [SerializeField] private float _durationSpeedBoost;
    // Para controlar boosts por jugador
    private static Dictionary<ulong, Coroutine> _activeBoostsGlobal = new Dictionary<ulong, Coroutine>();

    //TODO hacer con los bufos del sistema de daño 
    private IEnumerator BoostSpeedServerCoroutine(PlayerController playerController)
    {
        // Ajustamos la velocidad en el servidor
        playerController.SetSpeed(_speed);

        // Si tu movimiento es client-driven, avisa al cliente
        SetSpeedClientRpc(0, _speed);//playerController.OwnerClientId

        // Esperamos la duración del boost
        yield return new WaitForSeconds(_durationSpeedBoost);

        // Quitamos el boost en el servidor
        playerController.SetSpeed(-_speed);

        // Y avisamos al cliente de nuevo
        SetSpeedClientRpc(0, -_speed);//playerController.OwnerClientId

        // Lo sacamos del diccionario de boosts activos
        _activeBoostsGlobal.Remove(0);//playerController.OwnerClientId
    }

    [ClientRpc]
    private void SetSpeedClientRpc(ulong targetClientId, float speedDelta)
    {
        // Si este no es el cliente objetivo, no hacemos nada
        if (NetworkManager.Singleton.LocalClientId != targetClientId)
            return;

        // Aquí buscas tu PlayerController local. 
        // Esto dependerá de tu arquitectura; uno de los métodos es:
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

        // Obtenemos el PlayerController (o tu script de movimiento) 
        PlayerController playerController = other.GetComponentInChildren<PlayerController>();
        if (!playerController)
        {
            // Si no está en el root, prueba con GetComponentInChildren
            playerController = other.GetComponentInChildren<PlayerController>();
        }
        if (!playerController) return;
        // Sacamos el clientId de ese objeto
        ulong clientId = 0; //playerController.OwnerClientId

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
