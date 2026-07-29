using UnityEngine;
using Cysharp.Threading.Tasks;

public class PlayerEntity : Entity
{
    [SerializeField] private PlayerAnimationHandler _animationHandler;
    [SerializeField] private PlayerPositionHandler _positionHandler;

    protected override void OnBattleShown()
    {
        PlaySelectedPokemonAnim();
    }

    protected override void OnBattleHidden()
    {
        _animationHandler?.Stop();
        _positionHandler?.Reset();

        if (BattleManager.Instance != null)
            BattleManager.Instance.SetPlayerData(null);
    }

    protected override void OnDisable()
    {
        _animationHandler?.Stop();
        _positionHandler?.Reset();
        base.OnDisable();
    }

    private void PlaySelectedPokemonAnim()
    {
        if (GameManager.Instance == null || BattleManager.Instance == null)
            return;

        PokemonData data = GameManager.Instance.SelectedPokemonData;
        if (data == null)
            return;

        BattleManager.Instance.SetPlayerData(data);
        _positionHandler?.Apply(data);
        _animationHandler?.Play(data.BackAnimSprites, data.BackAnimDelays);
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
