using UnityEngine;
using UnityEngine.UI;

public class LoadingBarUI : AView<LoadingBarModel>
{
    [Header("Bar")]
    [SerializeField] private Image _loadingBar;

    private void OnEnable()
    {
        if (_model == null) return;

        _model.ProgressUpdated += HandleProgressUpdated;
        HandleProgressUpdated();
    }

    private void OnDisable()
    {
        if (_model == null) return;

        _model.ProgressUpdated -= HandleProgressUpdated;
    }

    private void HandleProgressUpdated()
    {
        if (_loadingBar == null) return;

        float progress = _model.TotalCount <= 0
            ? 0f
            : (float)_model.LoadedCount / _model.TotalCount;

        _loadingBar.fillAmount = progress;
    }
}
