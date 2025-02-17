using System;
using System.Collections.Generic;
using System.Linq;
using Unity.Netcode;

public class AbilityDataContainer : NetworkBehaviour, INetworkSerializable, IEquatable<AbilityDataContainer>
{
    List<IAbilityData> _dataList = new();
    public List<IAbilityData> DataList => _dataList;

    float _multiplier = 1f;
    public float Multiplier => _multiplier;

    public void AddInfo(IAbilityData info)
    {
        _dataList.Add(info);
        UpdateInfoList_ClientRpc(_dataList);
        //UpdateInfoList_ServerRpc(_infoList);
    }

    [Rpc(SendTo.ClientsAndHost)]
    private void UpdateInfoList_ClientRpc(List<IAbilityData> updatedList)
    {
        _dataList = new List<IAbilityData>(updatedList);
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
            serializer.GetFastBufferWriter().WriteValueSafe(_dataList);
        }
        else
        {
            serializer.GetFastBufferReader().ReadValueSafe(out _dataList);
        }
    }

    public bool Equals(AbilityDataContainer other)
    {
        if (other == null) return false;

        return _dataList.SequenceEqual(other._dataList) && _multiplier == other._multiplier;
    }

    public override bool Equals(object obj)
    {
        if (obj is AbilityDataContainer other) return Equals(other);

        return false;
    }

    public override int GetHashCode()
    {
        int hash = 17;
        foreach (var info in _dataList)
        {
            hash = hash * 31 + info.GetHashCode();
        }

        return hash;
    }
}