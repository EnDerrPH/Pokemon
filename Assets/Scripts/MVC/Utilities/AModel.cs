using System;

public abstract class AModel
{
    private bool _isVisible;

    public Action SubmitToggleVisibility;
    public Action SubmitHide;
    public Action SubmitShow;
    public Action VisibilityUpdated;
    public Action ShowComplete;
    public Action HideComplete;

    public bool IsVisible => _isVisible;

    public void SetVisibility(bool isVisible)
    {
        _isVisible = isVisible;
        VisibilityUpdated?.Invoke();
    }
}


