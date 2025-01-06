using System.Collections;
using System.Collections.Generic;
using Systems.ServiceLocator;
using Unity.Netcode;
using UnityEngine;

public enum Team
{
    A,
    B
}

public class CharacterSpawner : NetworkBehaviour
{
    [SerializeField] private CharacterDatabase _characterDatabase;
    [SerializeField] private NetworkObject _playerPrefab;
    [SerializeField] private Transform[] spawnPointsTeamA;
    [SerializeField] private Transform[] spawnPointsTeamB;
    private Dictionary<ulong, Team> _clientTeamDict = new Dictionary<ulong, Team>();

    private int _teamACount = 0;
    private int _teamBCount = 0;

    public override void OnNetworkSpawn()
    {
        if (!IsServer) return;

        SpawnAllPlayers();

    }

    private void SpawnAllPlayers()
    {
        foreach (var clientEntry in HostManager.Instance.ClientData)
        {
            ulong clientId = clientEntry.Value.clientId;

            AssignTeamToClient(clientId);

            var character = _characterDatabase.GetCharacterById(clientEntry.Value.characterId);
            if (character != null)
            {
                Team playerTeam = _clientTeamDict[clientId];
                Transform spawnTransform = GetSpawnTransformForTeam(playerTeam);

                NetworkObject instance = Instantiate(_playerPrefab, spawnTransform.position, spawnTransform.rotation);
                instance.SpawnAsPlayerObject(clientId);
            }
        }
    }

    private void AssignTeamToClient(ulong clientId)
    {
        if (_clientTeamDict.ContainsKey(clientId)) return;

        Team assignedTeam = (_teamACount <= _teamBCount) ? Team.A : Team.B;
        _clientTeamDict.Add(clientId, assignedTeam);

        if (assignedTeam == Team.A)
            _teamACount++;
        else
            _teamBCount++;
    }

    private Transform GetSpawnTransformForTeam(Team team)
    {
        if (team == Team.A)
        {
            int spawnIndex = (_teamACount - 1) % spawnPointsTeamA.Length;
            return spawnPointsTeamA[spawnIndex];
        }
        else
        {
            int spawnIndex = (_teamBCount - 1) % spawnPointsTeamB.Length;
            return spawnPointsTeamB[spawnIndex];
        }
    }
}