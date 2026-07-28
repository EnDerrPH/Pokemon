using UnityEngine;

public class AVisibilityController<Model> : AController<Model> where Model : AModel, new()
{
    protected virtual void OnEnable()
    {
        if (_model == null) return;

        _model.SubmitToggleVisibility += OnSubmitToggleVisibility;
        _model.SubmitHide += OnSubmitHide;
        _model.SubmitShow += OnSubmitShow;
    }

    protected virtual void OnDisable()
    {
        if (_model == null) return;

        _model.SubmitToggleVisibility -= OnSubmitToggleVisibility;
        _model.SubmitHide -= OnSubmitHide;
        _model.SubmitShow -= OnSubmitShow;
    }

    protected virtual void OnSubmitToggleVisibility()
    {
        _model.SetVisibility(!_model.IsVisible);
    }

    protected virtual void OnSubmitHide()
    {
        Hide();
    }

    protected virtual void OnSubmitShow()
    {
        Show();
    }

    protected void Show()
    {
        _model.SetVisibility(true);
    }

    protected void Hide()
    {
        _model.SetVisibility(false);
    }
}
