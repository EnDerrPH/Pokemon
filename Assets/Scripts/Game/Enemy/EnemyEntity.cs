using UnityEngine;

public class EnemyEntity : MonoBehaviour
{
    [SerializeField] private EnemyAnimationHandler _animationHandler;

    private BattleModel _battleModel;

    private void OnEnable()
    {
        BindBattleVisibility();

        if (_battleModel != null && _battleModel.IsVisible)
            PlayRandomEnemyAnim();
    }

    private void Start()
    {
        BindBattleVisibility();
    }

    private void OnDisable()
    {
        _animationHandler?.Stop();
        UnbindBattleVisibility();
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

        if (!_battleModel.IsVisible)
        {
            _animationHandler?.Stop();
            return;
        }

        PlayRandomEnemyAnim();
    }

    private void PlayRandomEnemyAnim()
    {
        if (GameManager.Instance == null || GameManager.Instance.PokemonDataList == null)
            return;

        int count = GameManager.Instance.PokemonDataList.Count;
        if (count <= 0)
            return;

        PokemonData enemyData = GameManager.Instance.PokemonDataList.GetByIndex(Random.Range(0, count));
        if (enemyData == null)
            return;

        _animationHandler?.Play(enemyData.FrontAnimSprites, enemyData.FrontAnimDelays);
    }
}
