using UnityEngine;

public class HealthController : AController<HealthModel>
{
    private enum HealthSide
    {
        Player,
        Enemy
    }

    [Header("Side")]
    [SerializeField] private HealthSide _side = HealthSide.Player;

    private void OnEnable()
    {
        BindBattleData();
    }

    private void Start()
    {
        BindBattleData();
    }

    private void OnDisable()
    {
        UnbindBattleData();
    }

    private void BindBattleData()
    {
        if (BattleManager.Instance == null)
            return;

        UnbindBattleData();

        if (_side == HealthSide.Player)
        {
            BattleManager.Instance.PlayerDataUpdated += HandleBattleDataUpdated;
            HandleBattleDataUpdated();
            return;
        }

        BattleManager.Instance.EnemyDataUpdated += HandleBattleDataUpdated;
        HandleBattleDataUpdated();
    }

    private void UnbindBattleData()
    {
        if (BattleManager.Instance == null)
            return;

        BattleManager.Instance.PlayerDataUpdated -= HandleBattleDataUpdated;
        BattleManager.Instance.EnemyDataUpdated -= HandleBattleDataUpdated;
    }

    private void HandleBattleDataUpdated()
    {
        if (_model == null || BattleManager.Instance == null)
            return;

        PokemonData data = _side == HealthSide.Player
            ? BattleManager.Instance.PlayerData
            : BattleManager.Instance.EnemyData;

        _model.SetFromPokemon(data);
    }
}
