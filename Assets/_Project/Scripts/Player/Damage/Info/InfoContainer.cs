using System;
using System.Collections.Generic;
using System.Linq;
using Unity.Netcode;

public class InfoContainer : NetworkBehaviour, INetworkSerializable, IEquatable<InfoContainer>
{
    private List<AbilityInfoTest> _infoList = new();
    public List<AbilityInfoTest> InfoList => _infoList;

    private float _multiplier = 1f;
    public float Multiplier => _multiplier;

    public void AddInfo(AbilityInfoTest info)
    {
        _infoList.Add(info);
        UpdateInfoList_ClientRpc(_infoList);
        //UpdateInfoList_ServerRpc(_infoList);
    }

    [Rpc(SendTo.ClientsAndHost)]
    private void UpdateInfoList_ClientRpc(List<AbilityInfoTest> updatedList)
    {
        _infoList = new List<AbilityInfoTest>(updatedList);
    }

    public void SetMultiplier(float multiplier)
    {
        UpdateMultiplier_ClientRpc(multiplier);
    }

    [Rpc(SendTo.ClientsAndHost)]
    private void UpdateMultiplier_ClientRpc(float multiplier)
    {
        _multiplier = multiplier;
    }

    public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
    {
        serializer.SerializeValue(ref _multiplier);
        if (serializer.IsWriter)
        {
            serializer.GetFastBufferWriter().WriteValueSafe(_infoList);
        }
        else
        {
            serializer.GetFastBufferReader().ReadValueSafe(out _infoList);
        }
    }

    public bool Equals(InfoContainer other)
    {
        if (other == null) return false;

        return _infoList.SequenceEqual(other._infoList) && _multiplier == other._multiplier;
    }

    public override bool Equals(object obj)
    {
        if (obj is InfoContainer other) return Equals(other);

        return false;
    }

    public override int GetHashCode()
    {
        int hash = 17;
        foreach (var info in _infoList)
        {
            hash = hash * 31 + info.GetHashCode();
        }

        return hash;
    }
}