using DG.Tweening;
using TMPro;
using UnityEngine;

public class StartUI : AView<LoadingBarModel>
{
    [Header("Text")]
    [SerializeField] private TextMeshProUGUI _text;

    [Header("Tween")]
    [SerializeField] private float _fadeDuration = 0.5f;

    private Tween _fadeTween;

    private void OnEnable()
    {
        if (_model == null) return;

        _model.LoadingComplete += HandleLoadingComplete;

        if (_model.TotalCount > 0 && _model.LoadedCount >= _model.TotalCount)
            HandleLoadingComplete();
    }

    private void OnDisable()
    {
        if (_model != null)
            _model.LoadingComplete -= HandleLoadingComplete;

        StopFadeLoop();
    }

    private void HandleLoadingComplete()
    {
        StartFadeLoop();
    }

    private void StartFadeLoop()
    {
        if (_text == null) return;

        StopFadeLoop();

        Color color = _text.color;
        color.a = 1f;
        _text.color = color;

        _fadeTween = _text
            .DOFade(0f, _fadeDuration)
            .SetEase(Ease.InOutSine)
            .SetLoops(-1, LoopType.Yoyo);
    }

    private void StopFadeLoop()
    {
        if (_fadeTween != null && _fadeTween.IsActive())
        {
            _fadeTween.Kill();
            _fadeTween = null;
        }
    }
}
