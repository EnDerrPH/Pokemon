using UnityEngine;
using Cysharp.Threading.Tasks;

public class EnemyEntity : Entity
{
    [SerializeField] private EnemyAnimationHandler _animationHandler;

    protected override void OnBattleShown()
    {
        PlayRandomEnemyAnim();
    }

    protected override void OnBattleHidden()
    {
        _animationHandler?.Stop();

        if (BattleManager.Instance != null)
            BattleManager.Instance.SetEnemyData(null);
    }

    protected override void OnDisable()
    {
        _animationHandler?.Stop();
        base.OnDisable();
    }

    private void PlayRandomEnemyAnim()
    {
        if (GameManager.Instance == null || BattleManager.Instance == null)
            return;

        if (GameManager.Instance.PokemonDataList == null)
            return;

        int count = GameManager.Instance.PokemonDataList.Count;
        if (count <= 0)
            return;

        PokemonData enemyData = GameManager.Instance.PokemonDataList.GetByIndex(Random.Range(0, count));
        if (enemyData == null)
            return;

        BattleManager.Instance.SetEnemyData(enemyData);
        _animationHandler?.Play(enemyData.FrontAnimSprites, enemyData.FrontAnimDelays);
    }

    public UniTask PlayJumpFx(float jumpHeight, float duration)
    {
        if (_animationHandler == null)
            return UniTask.CompletedTask;

        return _animationHandler.PlayJumpFx(jumpHeight, duration);
    }

    public UniTask PlayShakeFx(float duration, float strength)
    {
        if (_animationHandler == null)
            return UniTask.CompletedTask;

        return _animationHandler.PlayShakeFx(duration, strength);
    }
}
