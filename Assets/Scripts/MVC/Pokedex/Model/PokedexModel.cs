using System;

public class PokedexModel : AModel
{
    public PokemonListItemDTO[] PokemonList { get; private set; }
    public PokemonData SelectedPokemon { get; private set; }

    public Action PopulateButtons;
    public Action SelectedPokemonUpdated;

    public void SetPokemonList(PokemonListItemDTO[] pokemonList)
    {
        PokemonList = pokemonList;
    }

    public void SetSelectedPokemon(PokemonData pokemon)
    {
        SelectedPokemon = pokemon;
        SelectedPokemonUpdated?.Invoke();
    }
}
