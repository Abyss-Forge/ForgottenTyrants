using System.Collections.Generic;
using Unity.Netcode;

public static class AbilityInfoSerializationExtensions
{
    public static void WriteValueSafe(this FastBufferWriter writer, in List<AbilityInfo> list)
    {
        writer.WriteValueSafe(list.Count);
        foreach (var item in list)
        {
            writer.WriteValueSafe(item);
        }
    }

    public static void ReadValueSafe(this FastBufferReader reader, out List<AbilityInfo> list)
    {
        list = new List<AbilityInfo>();
        reader.ReadValueSafe(out int count);
        for (int i = 0; i < count; i++)
        {
            reader.ReadValueSafe(out AbilityInfo item);
            list.Add(item);
        }
    }
}
