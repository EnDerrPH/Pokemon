using System;
using UnityEngine;

public class GameManager : AMonoSingleton<GameManager>
{
    protected override bool DontDestroyOnLoad => true;

    [Header("Pokemon")]
    [SerializeField] private PokemonDataList _pokemonDataList;
    [SerializeField] private PokemonData _selectedPokemonData;

    [Header("Moves")]
    [SerializeField] private MoveDataList _moveDataList;

    private PokedexModel _pokedexModel;

    public PokemonDataList PokemonDataList => _pokemonDataList;
    public PokemonData SelectedPokemonData => _selectedPokemonData;
    public MoveDataList MoveDataList => _moveDataList;

    public Action SelectedPokemonUpdated;

    protected override void Awake()
    {
        base.Awake();
        Application.targetFrameRate = 60;
        EnsurePokemonDataList();
        EnsureMoveDataList();
    }

    protected override void OnSingletonInstantiated()
    {
        EnsurePokemonDataList();
        EnsureMoveDataList();
    }

    private void OnEnable()
    {
        BindPokedexSelection();
    }

    private void Start()
    {
        BindPokedexSelection();
    }

    private void OnDisable()
    {
        UnbindPokedexSelection();
    }

    public void SetSelectedPokemonData(PokemonData pokemonData)
    {
        _selectedPokemonData = pokemonData;
        SelectedPokemonUpdated?.Invoke();
    }

    private void BindPokedexSelection()
    {
        if (GlobalModelLocator.Instance == null)
            return;

        PokedexModel pokedexModel = GlobalModelLocator.Instance.GetModel<PokedexModel>();
        if (pokedexModel == null || _pokedexModel == pokedexModel)
            return;

        UnbindPokedexSelection();

        _pokedexModel = pokedexModel;
        _pokedexModel.SelectedPokemonUpdated += HandlePokedexSelectedPokemonUpdated;

        if (_pokedexModel.SelectedPokemon != null)
            SetSelectedPokemonData(_pokedexModel.SelectedPokemon);
    }

    private void UnbindPokedexSelection()
    {
        if (_pokedexModel == null)
            return;

        _pokedexModel.SelectedPokemonUpdated -= HandlePokedexSelectedPokemonUpdated;
        _pokedexModel = null;
    }

    private void HandlePokedexSelectedPokemonUpdated()
    {
        if (_pokedexModel == null)
            return;

        SetSelectedPokemonData(_pokedexModel.SelectedPokemon);
    }

    private void EnsurePokemonDataList()
    {
        if (_pokemonDataList == null)
            _pokemonDataList = ScriptableObject.CreateInstance<PokemonDataList>();
    }

    private void EnsureMoveDataList()
    {
        if (_moveDataList == null)
            _moveDataList = ScriptableObject.CreateInstance<MoveDataList>();
    }
}
