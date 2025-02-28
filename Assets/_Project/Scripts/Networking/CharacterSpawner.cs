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
    [SerializeField] private float _respawnTime = 5f, _spawnInvincibilityTime = 3f;

    private int _teamACount = 0, _teamBCount = 0;

    private EventBinding<PlayerDeathEvent> _playerDeathEventBinding;

    public override void OnNetworkSpawn()
    {
        if (!IsServer) return;

        SpawnAllPlayers();
    }

    void OnEnable()
    {
        _playerDeathEventBinding = new EventBinding<PlayerDeathEvent>(() => StartCoroutine(Respawn()));
        EventBus<PlayerDeathEvent>.Register(_playerDeathEventBinding);
    }

    void OnDisable()
    {
        EventBus<PlayerDeathEvent>.Deregister(_playerDeathEventBinding);
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
            tr = _spawnPointsTeamA[_teamACount++];
        }
        else    // if (teamId == 1)
        {
            tr = _spawnPointsTeamB[_teamBCount++];
        }
        return tr;
    }

    private IEnumerator Respawn()
    {
        yield return new WaitForSeconds(_respawnTime);
        EventBus<PlayerRespawnEvent>.Raise(new PlayerRespawnEvent
        {
            IsFirstSpawn = false,
            SpawnInvincibilityTime = _spawnInvincibilityTime,
            RespawnTime = _respawnTime
        });

        ServiceLocator.Global.Get(out PlayerInfo player).Get(out CharacterController characterController);

        player.InitializeBaseBuildData(player.ClientData);

        characterController.enabled = false;
        Transform tr;
        if (player.ClientData.TeamId == 0)
        {
            tr = _spawnPointsTeamA[Random.Range(0, 2)];
        }
        else
        {
            tr = _spawnPointsTeamB[Random.Range(0, 2)];
        }
        characterController.transform.CopyTransform(tr);
        characterController.enabled = true;
    }

}