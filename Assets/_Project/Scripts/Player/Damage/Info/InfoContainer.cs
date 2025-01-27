using System;
using System.Collections.Generic;
using System.Linq;
using Unity.Netcode;

public class InfoContainer : NetworkBehaviour, INetworkSerializable, IEquatable<InfoContainer>
{
    private List<AbilityInfo> _infoList = new();
    public List<AbilityInfo> InfoList => _infoList;

    public void Add(AbilityInfo info)
    {
        if (!IsOwner) return;

        _infoList.Add(info);
        UpdateInfoListClientRpc(_infoList);
    }

    [ClientRpc]
    private void UpdateInfoListClientRpc(List<AbilityInfo> updatedList)
    {
        _infoList = new List<AbilityInfo>(updatedList);
    }

    public virtual void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
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
