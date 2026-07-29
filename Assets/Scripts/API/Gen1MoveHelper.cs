using System.Collections.Generic;

public static class Gen1MoveHelper
{
    public static string[] ExtractMoveNames(PokemonDTO dto)
    {
        if (dto?.moves == null || dto.moves.Length == 0)
            return new string[0];

        HashSet<string> names = new HashSet<string>();

        for (int i = 0; i < dto.moves.Length; i++)
        {
            MoveSlotDTO slot = dto.moves[i];
            if (slot?.move == null || string.IsNullOrEmpty(slot.move.name))
                continue;

            if (slot.version_group_details == null)
                continue;

            for (int j = 0; j < slot.version_group_details.Length; j++)
            {
                string versionGroup = slot.version_group_details[j]?.version_group?.name;
                if (versionGroup == "red-blue" || versionGroup == "yellow")
                {
                    names.Add(slot.move.name);
                    break;
                }
            }
        }

        string[] result = new string[names.Count];
        names.CopyTo(result);
        return result;
    }

    public static bool IsGen1VersionGroup(string versionGroupName)
    {
        return versionGroupName == "red-blue" || versionGroupName == "yellow";
    }
}
