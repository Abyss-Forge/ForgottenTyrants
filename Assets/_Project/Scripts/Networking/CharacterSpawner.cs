using System.Collections;
using Systems.EventBus;
using Systems.ServiceLocator;
using Unity.Netcode;
using UnityEngine;
using Utils.Extensions;

public class CharacterSpawner : NetworkBehaviour
{
    [SerializeField] private NetworkObject _playerPrefab;
    [SerializeField] private Transform[] _spawnPointsTeamA, _spawnPointsTeamB;
    [SerializeField] private float _timeForRespawn = 5f;
    private EventBinding<PlayerDeathEvent> _playerDeathEventBinding;
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

    IEnumerator Respawn()
    {
        yield return new WaitForSeconds(_timeForRespawn);
        ServiceLocator.Global.Get(out CharacterController characterController);
        EventBus<PlayerRespawnEvent>.Raise(new PlayerRespawnEvent());
        ServiceLocator.Global.Get(out PlayerInfo player);

        characterController.enabled = false;

        if (player.ClientData.TeamId == 0)
        {
            characterController.transform.CopyTransform(_spawnPointsTeamA[Random.Range(0, 2)]);
        }
        else
        {
            characterController.transform.CopyTransform(_spawnPointsTeamB[Random.Range(0, 2)]);
        }
        characterController.enabled = true;

        player.InitializeBaseBuildData(player.ClientData);
        //TODO reactivar player animator
    }

    private void OnEnable()
    {
        _playerDeathEventBinding = new EventBinding<PlayerDeathEvent>(() => StartCoroutine(Respawn()));
        EventBus<PlayerDeathEvent>.Register(_playerDeathEventBinding);
    }

    private void OnDisable()
    {
        EventBus<PlayerDeathEvent>.Deregister(_playerDeathEventBinding);
    }

}