using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class PlayerRef : MonoBehaviour
{
    [field: SerializeField] public NetworkObject NetworkObject { get; private set; }
    [field: SerializeField] public GameObject PlayerCombat { get; private set; }
    [field: SerializeField] public GameObject ModelRoot { get; private set; }
    [field: SerializeField] public GameObject CameraRoot { get; private set; }
    [field: SerializeField] public Canvas MinimapIconsCanvas { get; private set; }
    [field: SerializeField] public AudioSource Audio { get; private set; }


    public void UpdateModel(GameObject newModel)
    {
        Vector3 pos = ModelRoot.transform.position;
        Destroy(ModelRoot);
        ModelRoot = Instantiate(newModel, pos, Quaternion.identity, transform);
    }

}