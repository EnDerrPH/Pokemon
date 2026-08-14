using UnityEngine;

public class BattleLogTextController : AController<BattleLogModel>
{
    private void OnEnable()
    {
        BindPhase();
    }

    private void Start()
    {
        BindPhase();
    }

    private void OnDisable()
    {
        UnbindPhase();
    }

    private void BindPhase()
    {
        if (BattleManager.Instance == null)
            return;

        UnbindPhase();
        BattleManager.Instance.PhaseUpdated += HandlePhaseUpdated;
        HandlePhaseUpdated(BattleManager.Instance.CurrentPhase);
    }

    private void UnbindPhase()
    {
        if (BattleManager.Instance == null)
            return;

        BattleManager.Instance.PhaseUpdated -= HandlePhaseUpdated;
    }

    private void HandlePhaseUpdated(BattlePhase phase)
    {
        if (_model == null)
            return;

        _model.TextUpdated?.Invoke(phase.ToString());
    }
}
