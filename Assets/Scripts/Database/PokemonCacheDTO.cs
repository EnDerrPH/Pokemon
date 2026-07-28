using System;

[Serializable]
public class PokemonCacheManifest
{
    public int cacheVersion;
    public int expectedCount;
    public PokemonListItemDTO[] listItems;
}

[Serializable]
public class PokemonCacheEntry
{
    public int id;
    public string pokemonName;
    public string[] types;
    public int hp;
    public int attack;
    public int defense;
    public int speed;
    public float[] frontAnimDelays;
    public float[] backAnimDelays;
    public bool hasIconPng;
    public bool hasFrontGif;
    public bool hasBackGif;
}
