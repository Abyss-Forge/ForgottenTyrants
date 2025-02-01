using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Netcode;
using UnityEngine;

public class GameController : NetworkBehaviour
{
    [SerializeField] private TMP_Text _timer;
    [SerializeField] private TMP_Text _starting;
    [SerializeField] private float _minAlpha = 0.3f;
    [SerializeField] private float _maxAlpha = 1f;
    [SerializeField] private float _velocity = 1f;
    [SerializeField] private string _prepareTime = "00:30";
    [SerializeField] private string _gameTime = "15:00";

    private NetworkVariable<float> currentTime = new NetworkVariable<float>(
        0f,
        NetworkVariableReadPermission.Everyone,
        NetworkVariableWritePermission.Server
    );
    private NetworkVariable<bool> countingDown = new NetworkVariable<bool>(
       false,
       NetworkVariableReadPermission.Everyone,
       NetworkVariableWritePermission.Server
   );

    private NetworkVariable<bool> gameStarted = new NetworkVariable<bool>(
        false,
        NetworkVariableReadPermission.Everyone,
        NetworkVariableWritePermission.Server
    );

    public override void OnNetworkSpawn()
    {
        if (IsServer)
        {
            currentTime.Value = TimeStringToSeconds(_prepareTime);
            countingDown.Value = true;
            gameStarted.Value = false;
        }

    }

    private void Update()
    {
        TimerClock();
        GameStartingAnimation();
    }

    void TimerClock()
    {
        // Solo el Servidor actualiza la cuenta regresiva
        if (IsServer && countingDown.Value)
        {
            currentTime.Value -= Time.deltaTime;

            if (currentTime.Value <= 0f && !gameStarted.Value)
            {
                StartGameServerRpc();

            }
            else if (currentTime.Value <= 0f && gameStarted.Value)
            {
                FinishGame();
            }
        }
        _timer.text = SecondsToTimeString(currentTime.Value);
    }

    [ServerRpc(RequireOwnership = false)]
    private void StartGameServerRpc()
    {
        gameStarted.Value = true;
        HideStartingTextClientRpc();
        ActivateMovementPlayers();
        currentTime.Value = TimeStringToSeconds(_gameTime);
        countingDown.Value = true;
        //PlayersToSpawn();
    }

    /*void PlayersToSpawn()
    {
        // Reposiciona a los players
        CharacterSpawner spawner = FindObjectOfType<CharacterSpawner>();
        if (spawner != null)
        {
            spawner.ResetPlayersPositions();
        }
    }*/

    void FinishGame()
    {
        currentTime.Value = 0f;
        countingDown.Value = false;
    }

    [ClientRpc]
    private void HideStartingTextClientRpc()
    {
        StartCoroutine(HideStartingText());
    }

    public void ActivateMovementPlayers()
    {
        foreach (var player in FindObjectsOfType<PlayerController>())
        {
            player.ActivateMovementClientRpc();
        }
    }

    IEnumerator HideStartingText()
    {
        yield return new WaitForSeconds(.5f);
        Color _actualColor = _starting.color;
        _actualColor.a = 0;
        _starting.color = _actualColor;
    }

    void GameStartingAnimation()
    {
        if (gameStarted.Value) return;
        // Mathf.PingPong genera un valor que oscila entre 0 y (alfaMaximo - alfaMinimo)
        // Al sumarle alfaMinimo, el valor oscila entre alfaMinimo y alfaMaximo.
        float newAlpha = Mathf.PingPong(Time.time * _velocity, _maxAlpha - _minAlpha) + _minAlpha;

        // Actualizamos el color del texto, conservando los valores RGB y modificando el alfa.
        Color _actualColor = _starting.color;
        _actualColor.a = newAlpha;
        _starting.color = _actualColor;

    }

    private float TimeStringToSeconds(string timeString)
    {
        string[] split = timeString.Split(':');
        if (split.Length != 2)
            return 0;

        int minutes = int.Parse(split[0]);
        int seconds = int.Parse(split[1]);
        return minutes * 60 + seconds;
    }

    // Convierte un valor en segundos a un string en formato "MM:SS"
    private string SecondsToTimeString(float totalSeconds)
    {
        int minutes = Mathf.FloorToInt(totalSeconds / 60f);
        int seconds = Mathf.FloorToInt(totalSeconds % 60f);
        return string.Format("{0:00}:{1:00}", minutes, seconds);
    }
}
