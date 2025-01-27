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
    [SerializeField] private Transform[] _spawnPointsTeamA, _spawnPointsTeamB;

    private Dictionary<ulong, Team> _clientTeamDictionary = new();
    private int _teamACount = 0, _teamBCount = 0;

    public override void OnNetworkSpawn()
    {
        if (!IsServer) return;

        SpawnAllPlayers();
    }

    private void SpawnAllPlayers()
    {
        foreach (var clientEntry in HostManager.Instance.ClientData)
        {
            ulong clientId = clientEntry.Value.ClientId;

            AssignTeamToClient(clientId);

            var character = _characterDatabase.GetById(clientEntry.Value.CharacterId);
            if (character != null)
            {
                Team playerTeam = _clientTeamDictionary[clientId];
                Transform spawnTransform = GetSpawnTransformForTeam(playerTeam);

                NetworkObject instance = Instantiate(_playerPrefab, spawnTransform.position, spawnTransform.rotation);
                instance.SpawnAsPlayerObject(clientId);
            }
        }
    }

    private void AssignTeamToClient(ulong clientId)
    {
        if (_clientTeamDictionary.ContainsKey(clientId)) return;

        Team assignedTeam = (_teamACount <= _teamBCount) ? Team.A : Team.B;
        _clientTeamDictionary.Add(clientId, assignedTeam);

        if (assignedTeam == Team.A)
            _teamACount++;
        else
            _teamBCount++;
    }

    private Transform GetSpawnTransformForTeam(Team team)
    {
        if (team == Team.A)
        {
            int spawnIndex = (_teamACount - 1) % _spawnPointsTeamA.Length;
            return _spawnPointsTeamA[spawnIndex];
        }
        else
        {
            int spawnIndex = (_teamBCount - 1) % _spawnPointsTeamB.Length;
            return _spawnPointsTeamB[spawnIndex];
        }
    }

    public void ResetPlayersPositions()
    {
        // Recorremos el diccionario que tiene (clientId -> Team)
        foreach (var kvp in _clientTeamDictionary)
        {
            ulong clientId = kvp.Key;
            Team team = kvp.Value;

            // Obtenemos el objeto player de ese client
            if (NetworkManager.Singleton.ConnectedClients.TryGetValue(clientId, out var clientData))
            {
                NetworkObject playerObject = clientData.PlayerObject;
                if (playerObject != null)
                {
                    // Calculamos el Transform de Spawn según el equipo
                    Transform spawnTransform = GetSpawnTransformForTeam(team);

                    // Obtenemos el CharacterController (si existe)
                    CharacterController cc = playerObject.GetComponent<CharacterController>();
                    if (cc != null && cc.enabled)
                    {
                        cc.enabled = false;
                    }

                    // Asignamos posición y rotación en el servidor
                    playerObject.transform.position = spawnTransform.position;
                    playerObject.transform.rotation = spawnTransform.rotation;

                    // Activamos nuevamente el CharacterController
                    if (cc != null)
                    {
                        cc.enabled = true;
                    }
                }
            }
        }
    }
}