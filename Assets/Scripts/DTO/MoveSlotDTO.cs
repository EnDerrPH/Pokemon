using System;

[Serializable]
public class MoveSlotDTO
{
    public NamedResourceDTO move;
    public MoveVersionGroupDetailDTO[] version_group_details;
}

[Serializable]
public class MoveVersionGroupDetailDTO
{
    public int level_learned_at;
    public NamedResourceDTO move_learn_method;
    public NamedResourceDTO version_group;
}
