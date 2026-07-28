using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class PokedexButtonUI : AView<PokedexModel>, IPointerClickHandler
{
    [Header("Selection")]
    [SerializeField] private Image _targetImage;
    [SerializeField] private Color _selectedColor = Color.green;

    private Color _defaultColor = Color.white;
    private bool _hasCachedDefaultColor;
    private PokemonData _pokemonData;

    public PokemonData PokemonData => _pokemonData;

    public Action PokemonDataUpdated;

    protected override void Awake()
    {
        base.Awake();

        if (_targetImage == null)
            _targetImage = GetComponent<Image>();

        Button button = GetComponent<Button>();
        if (button != null)
            button.transition = Selectable.Transition.None;

        CacheDefaultColor();
    }

    private void OnEnable()
    {
        if (_model != null)
            _model.SelectedPokemonUpdated += HandleSelectedPokemonUpdated;

        PokemonDataUpdated?.Invoke();
        RefreshSelectionVisual();
    }

    private void OnDisable()
    {
        if (_model != null)
            _model.SelectedPokemonUpdated -= HandleSelectedPokemonUpdated;
    }

    public void SetPokemon(PokemonData data)
    {
        _pokemonData = data;
        PokemonDataUpdated?.Invoke();
        RefreshSelectionVisual();
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (_pokemonData == null || _model == null)
            return;

        _model.SetSelectedPokemon(_pokemonData);
    }

    private void HandleSelectedPokemonUpdated()
    {
        RefreshSelectionVisual();
    }

    private void CacheDefaultColor()
    {
        if (_hasCachedDefaultColor || _targetImage == null)
            return;

        _defaultColor = _targetImage.color;
        _hasCachedDefaultColor = true;
    }

    private void RefreshSelectionVisual()
    {
        if (_targetImage == null)
            return;

        CacheDefaultColor();

        bool isSelected = _model != null
            && _pokemonData != null
            && _model.SelectedPokemon == _pokemonData;

        _targetImage.color = isSelected ? _selectedColor : _defaultColor;
    }
}
