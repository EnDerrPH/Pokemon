using UnityEngine;

public abstract class AVisibilityView<Model> : AView<Model> where Model : AModel, new()
{
    [Header("View")]
    [SerializeField] protected GameObject _view;

    public virtual void OnEnable()
    {
        if (_model == null) return;

        _model.VisibilityUpdated += OnVisibilityUpdated;
    }

    public virtual void OnDisable()
    {
        if (_model == null) return;

        _model.VisibilityUpdated -= OnVisibilityUpdated;
    }

    public virtual void OnVisibilityUpdated()
    {
        if (_model.IsVisible)
            Show();
        else
            Hide();
    }

    public virtual void Show()
    {
        if (_view != null)
            _view.SetActive(true);
    }

    public virtual void Hide()
    {
        if (_view != null)
            _view.SetActive(false);
    }
}
