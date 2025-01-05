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
            var character = _characterDatabase.GetById(client.Value.CharacterId);
            if (character != null)
            {
                Vector3 spawnPos = new(Random.Range(-3f, 3f), 0f, Random.Range(-3f, 3f));   // TODO: change spawn position

                NetworkObject instance = Instantiate(_playerPrefab, spawnPos, Quaternion.identity);
                instance.SpawnAsPlayerObject(client.Value.ClientId);
            }
        }
    }

}