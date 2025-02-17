using System;
using System.Collections.Generic;
using Unity.Netcode;

public static class AbilityDataSerializationExtensions
{
    public static void WriteValueSafe(this FastBufferWriter writer, in List<IAbilityData> list)
    {
        writer.WriteValueSafe(list.Count);
        foreach (var item in list)
        {
            byte typeId = item switch
            {
                DamageData => 1,
                HealData => 2,
                BuffData => 3,
                _ => throw new Exception("Unknown IAbilityData type encountered during serialization")
            };

            writer.WriteValueSafe(typeId); // Write the type identifier
            writer.WriteValueSafe(item);   // Write the actual data
        }
    }

    public static void ReadValueSafe(this FastBufferReader reader, out List<IAbilityData> list)
    {
        list = new List<IAbilityData>();
        reader.ReadValueSafe(out int count);

        for (int i = 0; i < count; i++)
        {
            reader.ReadValueSafe(out byte typeId);

            IAbilityData item = typeId switch
            {
                1 => ReadDamageData(reader),
                2 => ReadHealData(reader),
                3 => ReadBuffData(reader),
                _ => throw new Exception("Unknown IAbilityData type ID")
            };

            list.Add(item);
        }
    }

    private static DamageData ReadDamageData(FastBufferReader reader)
    {
        reader.ReadValueSafe(out ulong playerId);
        reader.ReadValueSafe(out int teamId);
        reader.ReadValueSafe(out EDamageApplyChannel affectedChannel);
        reader.ReadValueSafe(out float damageAmount);
        reader.ReadValueSafe(out EElementalType damageType);

        return new DamageData(playerId, teamId, affectedChannel, damageAmount, damageType);
    }

    private static HealData ReadHealData(FastBufferReader reader)
    {
        reader.ReadValueSafe(out ulong playerId);
        reader.ReadValueSafe(out int teamId);
        reader.ReadValueSafe(out EDamageApplyChannel affectedChannel);
        reader.ReadValueSafe(out float healAmount);

        return new HealData(playerId, teamId, affectedChannel, healAmount);
    }

    private static BuffData ReadBuffData(FastBufferReader reader)
    {
        reader.ReadValueSafe(out ulong playerId);
        reader.ReadValueSafe(out int teamId);
        reader.ReadValueSafe(out EDamageApplyChannel affectedChannel);
        reader.ReadValueSafe(out EStat stat);
        reader.ReadValueSafe(out float value);
        reader.ReadValueSafe(out bool isDebuff);
        reader.ReadValueSafe(out bool isPercentual);
        reader.ReadValueSafe(out float duration);

        return new BuffData(playerId, teamId, affectedChannel, stat, value, isDebuff, isPercentual, duration);
    }
}
