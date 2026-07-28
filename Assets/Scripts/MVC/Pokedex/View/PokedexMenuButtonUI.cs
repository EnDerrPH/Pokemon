using UnityEngine;
using UnityEngine.UI;

public class PokedexMenuButtonUI : AView<PokedexModel>
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
        _model.SubmitToggleVisibility?.Invoke();
    }
}
