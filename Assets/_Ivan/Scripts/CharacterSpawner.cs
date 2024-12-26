using System.Collections;
using System.Collections.Generic;
using Systems.ServiceLocator;
using Unity.Netcode;
using UnityEngine;

public class CharacterSpawner : NetworkBehaviour
{
    [SerializeField] private CharacterDatabase _characterDatabase;
    [SerializeField] private NetworkObject _playerPrefab;

    public override void OnNetworkSpawn()
    {
        if (!IsServer) return;

        foreach (var client in HostManager.Instance.ClientData)
        {
            var character = _characterDatabase.GetCharacterById(client.Value.characterId);
            if (character != null)
            {
                Vector3 spawnPos = new(Random.Range(-3f, 3f), 0f, Random.Range(-3f, 3f));

                //ServiceLocator.For(_playerPrefab).Get(out NetworkObject networkObject);

                NetworkObject instance = Instantiate(_playerPrefab, spawnPos, Quaternion.identity);
                //character.PlayerRef.UpdateModel(character.ModelRoot);
                instance.SpawnAsPlayerObject(client.Value.clientId);
            }
        }
    }

}