using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;

public class BattleFlowManager : AMonoSingleton<BattleFlowManager>
{
    protected override bool DontDestroyOnLoad => true;

    [SerializeField] private float _betweenActionsDelay = 1f;

    public async UniTask SubmitPlayerMove(MoveData playerMove)
    {
        if (BattleManager.Instance == null)
            return;

        if (BattleManager.Instance.CurrentPhase != BattlePhase.WaitingForInput)
            return;

        PokemonData playerData = BattleManager.Instance.PlayerData;
        PokemonData enemyData = BattleManager.Instance.EnemyData;
        if (playerMove == null || playerData == null || enemyData == null)
            return;

        BattleManager.Instance.SetPhase(BattlePhase.ResolvingTurn);

        MoveData enemyMove = PickRandomEnemyMove(enemyData);
        List<TurnAction> actions = BuildTurnActions(playerData, enemyData, playerMove, enemyMove);

        for (int i = 0; i < actions.Count; i++)
        {
            TurnAction action = actions[i];
            if (BattleAnimationManager.Instance != null)
                await BattleAnimationManager.Instance.PlayAttackPresentation(action.AttackerSide, action.DefenderSide);

            if (_betweenActionsDelay > 0f && i < actions.Count - 1)
                await UniTask.Delay(TimeSpan.FromSeconds(_betweenActionsDelay));
        }

        if (BattleManager.Instance.PlayerData == null || BattleManager.Instance.EnemyData == null)
        {
            BattleManager.Instance.SetPhase(BattlePhase.BattleOver);
            return;
        }

        BattleManager.Instance.SetPhase(BattlePhase.WaitingForInput);
    }

    private static MoveData PickRandomEnemyMove(PokemonData enemyData)
    {
        if (enemyData == null || enemyData.MoveNames == null || enemyData.MoveNames.Length == 0)
            return null;

        MoveDataList moveDataList = GameManager.Instance != null ? GameManager.Instance.MoveDataList : null;
        if (moveDataList == null)
            return null;

        List<MoveData> moves = new List<MoveData>();
        for (int i = 0; i < enemyData.MoveNames.Length; i++)
        {
            MoveData move = moveDataList.GetByName(enemyData.MoveNames[i]);
            if (move != null)
                moves.Add(move);
        }

        if (moves.Count == 0)
            return null;

        return moves[UnityEngine.Random.Range(0, moves.Count)];
    }

    private static List<TurnAction> BuildTurnActions(PokemonData playerData, PokemonData enemyData, MoveData playerMove, MoveData enemyMove)
    {
        List<TurnAction> actions = new List<TurnAction>();

        bool playerFirst = playerData != null && enemyData != null
            ? playerData.Speed >= enemyData.Speed
            : true;

        if (playerData != null && enemyData != null && playerData.Speed == enemyData.Speed)
            playerFirst = UnityEngine.Random.value > 0.5f;

        TurnAction playerAction = new TurnAction
        {
            AttackerSide = BattleSide.Player,
            DefenderSide = BattleSide.Enemy,
            Move = playerMove
        };

        TurnAction enemyAction = new TurnAction
        {
            AttackerSide = BattleSide.Enemy,
            DefenderSide = BattleSide.Player,
            Move = enemyMove
        };

        if (playerFirst)
        {
            if (playerMove != null)
                actions.Add(playerAction);

            if (enemyMove != null)
                actions.Add(enemyAction);

            return actions;
        }

        if (enemyMove != null)
            actions.Add(enemyAction);

        if (playerMove != null)
            actions.Add(playerAction);

        return actions;
    }
}
