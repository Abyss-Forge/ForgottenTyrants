using System;
using System.Collections.Generic;
using System.Linq;
using Unity.Netcode;

public class InfoContainer : NetworkBehaviour, INetworkSerializable, IEquatable<InfoContainer>
{
    private List<AbilityInfoTest> _infoList = new();
    public List<AbilityInfoTest> InfoList => _infoList;

    public void Add(AbilityInfoTest info)
    {
        _infoList.Add(info);
        UpdateInfoListServerRpc(_infoList);
    }

    [ServerRpc(RequireOwnership = false)]
    private void UpdateInfoListServerRpc(List<AbilityInfoTest> updatedList)
    {
        _infoList = new List<AbilityInfoTest>(updatedList);
        UpdateInfoListClientRpc(updatedList);
    }

    [ClientRpc]
    private void UpdateInfoListClientRpc(List<AbilityInfoTest> updatedList)
    {
        _infoList = new List<AbilityInfoTest>(updatedList);
    }

    public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
    {
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

        return _infoList.SequenceEqual(other._infoList);
    }

    public override bool Equals(object obj)
    {
        if (obj is InfoContainer other)
        {
            return Equals(other);
        }

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