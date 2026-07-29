using UnityEngine;

public abstract class Entity : MonoBehaviour
{
    private BattleModel _battleModel;

    protected virtual void OnEnable()
    {
        BindBattleVisibility();

        if (_battleModel != null && _battleModel.IsVisible)
            OnBattleShown();
    }

    protected virtual void Start()
    {
        BindBattleVisibility();
    }

    protected virtual void OnDisable()
    {
        OnBattleHidden();
        UnbindBattleVisibility();
    }

    protected virtual void OnBattleShown()
    {
    }

    protected virtual void OnBattleHidden()
    {
    }

    private void BindBattleVisibility()
    {
        if (GlobalModelLocator.Instance == null)
            return;

        BattleModel battleModel = GlobalModelLocator.Instance.GetModel<BattleModel>();
        if (battleModel == null || _battleModel == battleModel)
            return;

        UnbindBattleVisibility();

        _battleModel = battleModel;
        _battleModel.VisibilityUpdated += HandleBattleVisibilityUpdated;
    }

    private void UnbindBattleVisibility()
    {
        if (_battleModel == null)
            return;

        _battleModel.VisibilityUpdated -= HandleBattleVisibilityUpdated;
        _battleModel = null;
    }

    private void HandleBattleVisibilityUpdated()
    {
        if (_battleModel == null)
            return;

        if (_battleModel.IsVisible)
            OnBattleShown();
        else
            OnBattleHidden();
    }
}
