using Unity.Netcode;
using UnityEngine;

public class CharacterSpawner : NetworkBehaviour
{
    [SerializeField] private NetworkObject _playerPrefab;
    [SerializeField] private Transform[] _spawnPointsTeamA, _spawnPointsTeamB;

    private int _teamACount = 0, _teamBCount = 0;

    public override void OnNetworkSpawn()
    {
        if (!IsServer) return;

        SpawnAllPlayers();
    }

    private void SpawnAllPlayers()
    {
        foreach (var clientEntry in HostManager.Instance.ClientDataDict)
        {
            var character = clientEntry.Value.Character;
            if (character != null)
            {
                Transform spawnTransform = GetSpawnTransformForTeam(clientEntry.Value.TeamId);

                NetworkObject instance = Instantiate(_playerPrefab, spawnTransform.position, spawnTransform.rotation);
                instance.SpawnAsPlayerObject(clientEntry.Value.ClientId);
            }
        }
    }

    private Transform GetSpawnTransformForTeam(int teamId)
    {
        Transform tr;
        if (teamId == 0)
        {
            tr = _spawnPointsTeamA[_teamACount];
            _teamACount++;
        }
        else
        {
            tr = _spawnPointsTeamB[_teamBCount];
            _teamBCount++;
        }
        return tr;
    }

}