using TMPro;
using UnityEngine;

public class LoadingTextUI : AView<LoadingBarModel>
{
    [Header("Text")]
    [SerializeField] private TextMeshProUGUI _loadingText;

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
        if (_loadingText == null) return;

        float progress = _model.TotalCount <= 0
            ? 0f
            : (float)_model.LoadedCount / _model.TotalCount;

        int percent = Mathf.RoundToInt(progress * 100f);
        _loadingText.SetText($"{percent}%");
    }
}
