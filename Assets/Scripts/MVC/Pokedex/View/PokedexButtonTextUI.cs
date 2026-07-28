using TMPro;
using UnityEngine;

public class PokedexButtonTextUI : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private PokedexButtonUI _root;
    [SerializeField] private TextMeshProUGUI _text;

    private void OnEnable()
    {
        if (_root == null) return;

        _root.PokemonDataUpdated += HandlePokemonDataUpdated;
        HandlePokemonDataUpdated();
    }

    private void OnDisable()
    {
        if (_root == null) return;

        _root.PokemonDataUpdated -= HandlePokemonDataUpdated;
    }

private void HandlePokemonDataUpdated()
    {
        if (_text == null || _root == null || _root.PokemonData == null)
            return;

        PokemonData data = _root.PokemonData;
        _text.SetText($"{data.Id}. {data.PokemonName.ToUpperInvariant()}");
    }
}
