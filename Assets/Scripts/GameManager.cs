using UnityEngine;

public class GameManager : AMonoSingleton<GameManager>
{
    protected override bool DontDestroyOnLoad => true;

    [Header("Pokemon")]
    [SerializeField] private PokemonDataList _pokemonDataList;

    public PokemonDataList PokemonDataList => _pokemonDataList;

    protected override void Awake()
    {
        base.Awake();
        EnsurePokemonDataList();
    }

    protected override void OnSingletonInstantiated()
    {
        EnsurePokemonDataList();
    }

    private void EnsurePokemonDataList()
    {
        if (_pokemonDataList == null)
            _pokemonDataList = ScriptableObject.CreateInstance<PokemonDataList>();
    }
}
