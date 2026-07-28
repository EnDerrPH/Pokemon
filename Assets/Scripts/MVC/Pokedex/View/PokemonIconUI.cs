using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class PokemonIconUI : AView<PokedexModel>
{
    [Header("Image")]
    [SerializeField] private Image _image;

    [Header("Animation")]
    [SerializeField] private Vector3 _punchScale = new Vector3(0.2f, 0.2f, 0f);
    [SerializeField] private float _punchDuration = 0.3f;
    [SerializeField] private int _punchVibrato = 8;
    [SerializeField] private float _punchElasticity = 0.5f;

    private void OnEnable()
    {
        if (_model == null) return;

        _model.SelectedPokemonUpdated += HandleSelectedPokemonUpdated;
        HandleSelectedPokemonUpdated();
    }

    private void OnDisable()
    {
        if (_model == null) return;

        _model.SelectedPokemonUpdated -= HandleSelectedPokemonUpdated;

        if (_image != null)
        {
            _image.transform.DOKill();
            _image.transform.localScale = Vector3.one;
        }
    }

    private void HandleSelectedPokemonUpdated()
    {
        if (_image == null || _model.SelectedPokemon == null)
            return;

        _image.sprite = _model.SelectedPokemon.PokemonIcon;
        PlayPunchScale();
    }

    private void PlayPunchScale()
    {
        Transform target = _image.transform;
        target.DOKill();
        target.localScale = Vector3.one;
        target.DOPunchScale(_punchScale, _punchDuration, _punchVibrato, _punchElasticity);
    }
}
