using Cysharp.Threading.Tasks;
using UnityEngine;

public class BattleAnimationManager : AMonoSingleton<BattleAnimationManager>
{
    protected override bool DontDestroyOnLoad => true;

    [Header("Battle FX")]
    [SerializeField] private float _jumpHeight = 0.35f;
    [SerializeField] private float _jumpDuration = 0.2f;
    [SerializeField] private float _shakeDuration = 1.5f;
    [SerializeField] private float _shakeStrength = 0.12f;

    [Header("Runtime References")]
    [SerializeField] private PlayerEntity _playerEntity;
    [SerializeField] private EnemyEntity _enemyEntity;

    public async UniTask PlayAttackPresentation(BattleSide attacker, BattleSide defender)
    {
        EnsureEntities();

        if (attacker == BattleSide.Player && _playerEntity != null)
            await _playerEntity.PlayJumpFx(_jumpHeight, _jumpDuration);
        else if (attacker == BattleSide.Enemy && _enemyEntity != null)
            await _enemyEntity.PlayJumpFx(_jumpHeight, _jumpDuration);

        if (defender == BattleSide.Player && _playerEntity != null)
            await _playerEntity.PlayShakeFx(_shakeDuration, _shakeStrength);
        else if (defender == BattleSide.Enemy && _enemyEntity != null)
            await _enemyEntity.PlayShakeFx(_shakeDuration, _shakeStrength);
    }

    private void EnsureEntities()
    {
        if (_playerEntity == null)
            _playerEntity = FindAnyObjectByType<PlayerEntity>();

        if (_enemyEntity == null)
            _enemyEntity = FindAnyObjectByType<EnemyEntity>();
    }
}
