public class BattleLogVisibilityController : AVisibilityController<BattleLogModel>
{
    private BattleModel _battleModel;

    protected override void OnEnable()
    {
        base.OnEnable();
        BindBattleVisibility();
    }

    private void Start()
    {
        BindBattleVisibility();
    }

    protected override void OnDisable()
    {
        UnbindBattleVisibility();
        base.OnDisable();
    }

    private void BindBattleVisibility()
    {
        if (GlobalModelLocator.Instance == null)
            return;

        UnbindBattleVisibility();

        _battleModel = GlobalModelLocator.Instance.GetModel<BattleModel>();
        if (_battleModel == null)
            return;

        _battleModel.VisibilityUpdated += HandleBattleVisibilityUpdated;
        HandleBattleVisibilityUpdated();
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
        if (_model == null || _battleModel == null)
            return;

        _model.SetVisibility(_battleModel.IsVisible);
    }
}
