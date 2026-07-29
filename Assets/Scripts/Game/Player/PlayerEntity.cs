using UnityEngine;

public class PlayerEntity : MonoBehaviour
{
    [SerializeField] private PlayerAnimationHandler _animationHandler;
    [SerializeField] private PlayerPositionHandler _positionHandler;

    private BattleModel _battleModel;

    private void OnEnable()
    {
        BindBattleVisibility();

        if (_battleModel != null && _battleModel.IsVisible)
            PlaySelectedPokemonAnim();
    }

    private void Start()
    {
        BindBattleVisibility();
    }

    private void OnDisable()
    {
        _animationHandler?.Stop();
        _positionHandler?.Reset();
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
            _positionHandler?.Reset();
            return;
        }

        PlaySelectedPokemonAnim();
    }

    private void PlaySelectedPokemonAnim()
    {
        if (GameManager.Instance == null || GameManager.Instance.SelectedPokemonData == null)
            return;

        PokemonData data = GameManager.Instance.SelectedPokemonData;
        _positionHandler?.Apply(data);
        _animationHandler?.Play(data.BackAnimSprites, data.BackAnimDelays);
    }
}
