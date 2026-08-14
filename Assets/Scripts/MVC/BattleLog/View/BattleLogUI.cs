using TMPro;
using UnityEngine;

public class BattleLogUI : BattleLogView
{
    [Header("Log")]
    [SerializeField] private TextMeshProUGUI _logText;

    private void OnEnable()
    {
        if (_model == null)
            BindModel(_modelName);

        if (_model == null)
            return;

        _model.TextUpdated -= HandleTextUpdated;
        _model.TextUpdated += HandleTextUpdated;
    }

    private void OnDisable()
    {
        if (_model == null)
            return;

        _model.TextUpdated -= HandleTextUpdated;
    }

    private void HandleTextUpdated(string text)
    {
        if (_logText == null)
            return;

        _logText.SetText(text ?? string.Empty);
    }
}

