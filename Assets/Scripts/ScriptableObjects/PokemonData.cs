using UnityEngine;

[CreateAssetMenu(menuName = "Pokemon/Pokemon Data")]
public class PokemonData : ScriptableObject
{
    [SerializeField] private int _id;
    [SerializeField] private string _pokemonName;
    [SerializeField] private Sprite _pokemonIcon;
    [SerializeField] private Sprite _frontSprite;
    [SerializeField] private Sprite _backSprite;
    [SerializeField] private Sprite[] _frontAnimSprites;
    [SerializeField] private Sprite[] _backAnimSprites;
    [SerializeField] private float[] _frontAnimDelays;
    [SerializeField] private float[] _backAnimDelays;
    [SerializeField] private string[] _types;
    [SerializeField] private int _hp;
    [SerializeField] private int _attack;
    [SerializeField] private int _defense;
    [SerializeField] private int _speed;

    public int Id => _id;
    public string PokemonName => _pokemonName;
    public Sprite PokemonIcon => _pokemonIcon;
    public Sprite FrontSprite => _frontSprite;
    public Sprite BackSprite => _backSprite;
    public Sprite Sprite => _frontSprite;
    public Sprite[] FrontAnimSprites => _frontAnimSprites;
    public Sprite[] BackAnimSprites => _backAnimSprites;
    public float[] FrontAnimDelays => _frontAnimDelays;
    public float[] BackAnimDelays => _backAnimDelays;
    public string[] Types => _types;
    public int Hp => _hp;
    public int Attack => _attack;
    public int Defense => _defense;
    public int Speed => _speed;

    public void Initialize(
        int id,
        string pokemonName,
        Sprite pokemonIcon,
        Sprite frontSprite,
        Sprite backSprite,
        Sprite[] frontAnimSprites,
        Sprite[] backAnimSprites,
        float[] frontAnimDelays,
        float[] backAnimDelays,
        string[] types,
        int hp,
        int attack,
        int defense,
        int speed)
    {
        _id = id;
        _pokemonName = pokemonName;
        _pokemonIcon = pokemonIcon;
        _frontSprite = frontSprite;
        _backSprite = backSprite;
        _frontAnimSprites = frontAnimSprites;
        _backAnimSprites = backAnimSprites;
        _frontAnimDelays = frontAnimDelays;
        _backAnimDelays = backAnimDelays;
        _types = types;
        _hp = hp;
        _attack = attack;
        _defense = defense;
        _speed = speed;
    }
}
