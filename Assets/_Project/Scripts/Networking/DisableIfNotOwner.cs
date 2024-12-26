using Unity.Netcode;

public class DisableIfNotOwner : NetworkBehaviour
{
    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();

        if (!IsOwner) gameObject.SetActive(false);
    }

}