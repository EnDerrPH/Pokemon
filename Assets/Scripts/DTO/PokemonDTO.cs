using System;

[Serializable]
public class PokemonDTO
{
    public int id;

    public string name;

    public SpriteDTO sprites;

    public TypeSlotDTO[] types;

    public StatDTO[] stats;

    public MoveSlotDTO[] moves;
}
