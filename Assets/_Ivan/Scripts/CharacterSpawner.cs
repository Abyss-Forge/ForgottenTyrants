using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class CharacterSpawner : NetworkBehaviour
{
    [SerializeField] private CharacterDatabase _characterDatabase;

    public override void OnNetworkSpawn()
    {
        if (!IsServer) return;

        foreach (var client in HostManager.Instance.ClientData)
        {
            var character = _characterDatabase.GetCharacterById(client.Value.characterId);
            if (character != null)
            {
                Vector3 spawnPos = new(Random.Range(-3f, 3f), 0f, Random.Range(-3f, 3f));
                NetworkObject instance = Instantiate(character.PlayerRef.NetworkObject, spawnPos, Quaternion.identity);
                //Destroy(character.PlayerRef.ModelRoot);
                //character.PlayerRef.ModelRoot = Instantiate(character.ModelRoot, Vector3.zero, Quaternion.identity, character.PlayerRef.transform);
                instance.SpawnAsPlayerObject(client.Value.clientId);
            }
        }
    }

}