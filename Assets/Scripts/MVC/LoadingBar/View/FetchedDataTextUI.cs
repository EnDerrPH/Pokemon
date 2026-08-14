using DG.Tweening;
using TMPro;
using UnityEngine;

public class FetchedDataTextUI : AView<LoadingBarModel>
{
    [Header("Text")]
    [SerializeField] private TextMeshProUGUI _fetchedDataText;

    [Header("Tween")]
    [SerializeField] private float _fadeOutDuration = 0.35f;

    private Tween _fadeTween;

    private void OnEnable()
    {
        if (_model == null) return;

        _model.ProgressUpdated += HandleProgressUpdated;
        _model.LoadingComplete += HandleLoadingComplete;

        HandleProgressUpdated();

        if (_model.TotalCount > 0 && _model.LoadedCount >= _model.TotalCount)
            HandleLoadingComplete();
    }

    private void OnDisable()
    {
        if (_model != null)
        {
            _model.ProgressUpdated -= HandleProgressUpdated;
            _model.LoadingComplete -= HandleLoadingComplete;
        }

        KillFade();
    }

    private void HandleProgressUpdated()
    {
        if (_fetchedDataText == null) return;

        string label = GetPhaseLabel(_model.FetchPhase);
        _fetchedDataText.SetText(label.ToLowerInvariant());
    }

    private void HandleLoadingComplete()
    {
        if (_fetchedDataText == null) return;

        KillFade();
        _fadeTween = _fetchedDataText
            .DOFade(0f, _fadeOutDuration)
            .SetEase(Ease.OutSine);
    }

    private void KillFade()
    {
        if (_fadeTween != null && _fadeTween.IsActive())
        {
            _fadeTween.Kill();
            _fadeTween = null;
        }
    }

    private static string GetPhaseLabel(LoadingFetchPhase phase)
    {
        switch (phase)
        {
            case LoadingFetchPhase.Pokemon:
                return "fetching pokemon data";
            case LoadingFetchPhase.Moves:
                return "fetching move data";
            default:
                return string.Empty;
        }
    }
}
