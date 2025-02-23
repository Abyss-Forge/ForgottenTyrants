using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class MinimapSync : NetworkBehaviour
{
    [SerializeField] private Image _arrowMarker;
    [SerializeField] private Color _myColor, _team1Color, _team2Color;

    public override void OnNetworkSpawn()
    {
        if (IsOwner)
        {
            _arrowMarker.color = _myColor;
        }
        else
        {
            ClientData data = HostManager.Instance.GetMyClientData();
            if (data.TeamId == 0)
            {
                _arrowMarker.color = _team1Color;
            }
            else
            {
                _arrowMarker.color = _team2Color;
            }
        }
    }

}