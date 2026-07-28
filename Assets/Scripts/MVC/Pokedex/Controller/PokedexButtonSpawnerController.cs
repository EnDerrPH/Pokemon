using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;

public class PokedexButtonSpawnerController : AController<PokedexModel>
{
    private readonly PokemonApi _api = new();

    private void OnEnable()
    {
        if (_model == null) return;

        _model.VisibilityUpdated += HandleVisibilityUpdated;
    }

    private void OnDisable()
    {
        if (_model == null) return;

        _model.VisibilityUpdated -= HandleVisibilityUpdated;
    }

    private void HandleVisibilityUpdated()
    {
        if (!_model.IsVisible)
            return;

        LoadAndPopulateAsync().Forget();
    }

    private async UniTaskVoid LoadAndPopulateAsync()
    {
        PokemonListDTO list = await _api.GetPokemonList();
        if (list?.results == null || list.results.Length == 0)
        {
            Debug.LogError("Failed to load Pokemon list");
            return;
        }

        _model.SetPokemonList(list.results);
        _model.PopulateButtons?.Invoke();
        SelectFirstPokemon();
    }

    private void SelectFirstPokemon()
    {
        if (GameManager.Instance == null || GameManager.Instance.PokemonDataList == null)
            return;

        IReadOnlyList<PokemonData> pokemonList = GameManager.Instance.PokemonDataList.GetAll();
        if (pokemonList == null || pokemonList.Count == 0)
            return;

        _model.SetSelectedPokemon(pokemonList[0]);
    }
}
