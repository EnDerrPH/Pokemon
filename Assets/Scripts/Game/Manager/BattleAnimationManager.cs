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

    [Header("Status FX")]
    [SerializeField] private Vector3 _punchScale = new Vector3(0.2f, 0.2f, 0f);
    [SerializeField] private float _punchDuration = 0.35f;
    [SerializeField] private int _punchVibrato = 8;
    [SerializeField] private float _punchElasticity = 0.5f;

    [Header("Runtime References")]
    [SerializeField] private PlayerEntity _playerEntity;
    [SerializeField] private EnemyEntity _enemyEntity;

    public async UniTask PlayAttackPresentation(BattleSide attacker, BattleSide defender, bool dealsDamage)
    {
        EnsureEntities();

        if (dealsDamage)
            await PlayJumpFx(attacker);
        else
            await PlayPunchScaleFx(attacker);

        if (!dealsDamage)
            return;

        if (defender == BattleSide.Player && _playerEntity != null)
            await _playerEntity.PlayShakeFx(_shakeDuration, _shakeStrength);
        else if (defender == BattleSide.Enemy && _enemyEntity != null)
            await _enemyEntity.PlayShakeFx(_shakeDuration, _shakeStrength);
    }

    private async UniTask PlayJumpFx(BattleSide attacker)
    {
        if (attacker == BattleSide.Player && _playerEntity != null)
            await _playerEntity.PlayJumpFx(_jumpHeight, _jumpDuration);
        else if (attacker == BattleSide.Enemy && _enemyEntity != null)
            await _enemyEntity.PlayJumpFx(_jumpHeight, _jumpDuration);
    }

    private async UniTask PlayPunchScaleFx(BattleSide attacker)
    {
        if (attacker == BattleSide.Player && _playerEntity != null)
            await _playerEntity.PlayPunchScaleFx(_punchScale, _punchDuration, _punchVibrato, _punchElasticity);
        else if (attacker == BattleSide.Enemy && _enemyEntity != null)
            await _enemyEntity.PlayPunchScaleFx(_punchScale, _punchDuration, _punchVibrato, _punchElasticity);
    }

    private void EnsureEntities()
    {
        if (_playerEntity == null)
            _playerEntity = FindAnyObjectByType<PlayerEntity>();

        if (_enemyEntity == null)
            _enemyEntity = FindAnyObjectByType<EnemyEntity>();
    }
}
