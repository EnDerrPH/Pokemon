using UnityEngine;
using UnityEngine.UI;

public class BattleMenuButtonUI : AView<BattleModel>
{
    [Header("Button")]
    [SerializeField] private Button _button;

    private void OnEnable()
    {
        if (_button != null)
            _button.onClick.AddListener(HandleClicked);
    }

    private void OnDisable()
    {
        if (_button != null)
            _button.onClick.RemoveListener(HandleClicked);
    }

    private void HandleClicked()
    {
        if (_model == null)
            return;

        _model.SubmitToggleVisibility?.Invoke();
    }
}
