using System;
using UnityEngine;

public class BattleManager : AMonoSingleton<BattleManager>
{
    protected override bool DontDestroyOnLoad => true;

    [Header("Battle Pokemon")]
    [SerializeField] private PokemonData _playerData;
    [SerializeField] private PokemonData _enemyData;
    [SerializeField] private BattlePhase _currentPhase = BattlePhase.WaitingForInput;

    public PokemonData PlayerData => _playerData;
    public PokemonData EnemyData => _enemyData;
    public BattlePhase CurrentPhase => _currentPhase;

    public Action PlayerDataUpdated;
    public Action EnemyDataUpdated;
    public Action<BattlePhase> PhaseUpdated;

    public void SetPhase(BattlePhase phase)
    {
        if (_currentPhase == phase)
            return;

        _currentPhase = phase;
        PhaseUpdated?.Invoke(_currentPhase);
    }

    public void SetPlayerData(PokemonData pokemonData)
    {
        _playerData = pokemonData;
        PlayerDataUpdated?.Invoke();
    }

    public void SetEnemyData(PokemonData pokemonData)
    {
        _enemyData = pokemonData;
        EnemyDataUpdated?.Invoke();
    }

    public void ClearBattleData()
    {
        SetPlayerData(null);
        SetEnemyData(null);
        SetPhase(BattlePhase.WaitingForInput);
    }
}
