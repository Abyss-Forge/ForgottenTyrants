using System.Collections.Generic;
using Unity.Netcode;

public static class AbilityInfoSerializationExtensions
{
    public static void WriteValueSafe(this FastBufferWriter writer, in List<AbilityInfoTest> list)
    {
        writer.WriteValueSafe(list.Count);
        foreach (var item in list)
        {
            writer.WriteValueSafe(item);
        }
    }

    public static void ReadValueSafe(this FastBufferReader reader, out List<AbilityInfoTest> list)
    {
        list = new List<AbilityInfoTest>();
        reader.ReadValueSafe(out int count);
        for (int i = 0; i < count; i++)
        {
            reader.ReadValueSafe(out AbilityInfoTest item);
            list.Add(item);
        }
    }
}
