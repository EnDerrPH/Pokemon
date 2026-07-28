using System.Collections.Generic;
using UnityEngine;

public class PokedexButtonSpawnerUI : AView<PokedexModel>
{
    [Header("Spawn")]
    [SerializeField] private PokedexButtonUI _buttonPrefab;
    [SerializeField] private Transform _content;

    private void OnEnable()
    {
        if (_model == null) return;

        _model.PopulateButtons += PopulateButtons;
    }

    private void OnDisable()
    {
        if (_model == null) return;

        _model.PopulateButtons -= PopulateButtons;
    }

    public void PopulateButtons()
    {
        if (_buttonPrefab == null || _content == null)
            return;

        if (GameManager.Instance == null || GameManager.Instance.PokemonDataList == null)
            return;

        IReadOnlyList<PokemonData> pokemonList = GameManager.Instance.PokemonDataList.GetAll();
        if (pokemonList == null || pokemonList.Count == 0)
            return;

        ClearContent();

        for (int i = 0; i < pokemonList.Count; i++)
        {
            PokedexButtonUI button = Instantiate(_buttonPrefab, _content, false);
            button.SetPokemon(pokemonList[i]);
        }
    }

    private void ClearContent()
    {
        for (int i = _content.childCount - 1; i >= 0; i--)
        {
            Destroy(_content.GetChild(i).gameObject);
        }
    }
}
