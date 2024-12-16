using System.Collections;
using System.Collections.Generic;
using Systems.EventBus;
using Systems.GameManagers;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerRef : MonoBehaviour
{
    [field: SerializeField] public NetworkObject NetworkObject { get; private set; }
    [field: SerializeField] public GameObject PlayerCombat { get; private set; }
    [SerializeField] public GameObject ModelRoot { get => ModelRoot; set { ModelRoot = value; UpdateModel(); } }
    [field: SerializeField] public GameObject CameraRoot { get; private set; }
    [field: SerializeField] public Canvas MinimapIconsCanvas { get; private set; }
    [field: SerializeField] public AudioSource Audio { get; private set; }


    private void UpdateModel()
    {

    }

}