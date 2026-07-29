using System;
using System.Collections.Generic;
using UnityEngine;

public class ExclusiveMenuVisibilityController : MonoBehaviour
{
    private readonly List<AModel> _models = new();
    private readonly Dictionary<AModel, Action> _handlers = new();
    private bool _isUpdating;

    private void Awake()
    {
        if (GlobalModelLocator.Instance == null)
            return;

        Register(GlobalModelLocator.Instance.GetModel<PokedexModel>());
        Register(GlobalModelLocator.Instance.GetModel<BattleModel>());
    }

    private void OnEnable()
    {
        for (int i = 0; i < _models.Count; i++)
        {
            AModel model = _models[i];
            if (!_handlers.ContainsKey(model))
            {
                AModel captured = model;
                _handlers[captured] = () => HandleVisibilityUpdated(captured);
            }

            model.VisibilityUpdated += _handlers[model];
        }
    }

    private void OnDisable()
    {
        for (int i = 0; i < _models.Count; i++)
        {
            AModel model = _models[i];
            if (_handlers.TryGetValue(model, out Action handler))
                model.VisibilityUpdated -= handler;
        }
    }

    private void Register(AModel model)
    {
        if (model == null || _models.Contains(model))
            return;

        _models.Add(model);
    }

    private void HandleVisibilityUpdated(AModel changedModel)
    {
        if (_isUpdating || changedModel == null || !changedModel.IsVisible)
            return;

        _isUpdating = true;

        for (int i = 0; i < _models.Count; i++)
        {
            AModel model = _models[i];
            if (model == changedModel)
                continue;

            if (model.IsVisible)
                model.SetVisibility(false);
        }

        _isUpdating = false;
    }
}
