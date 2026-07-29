using System.Collections.Generic;
using UnityEngine;

public class MovesController : AController<MovesModel>
{
    private void OnEnable()
    {
        BindPlayerData();
    }

    private void Start()
    {
        BindPlayerData();
    }

    private void OnDisable()
    {
        UnbindPlayerData();
    }

    private void BindPlayerData()
    {
        if (BattleManager.Instance == null)
            return;

        BattleManager.Instance.PlayerDataUpdated -= HandlePlayerDataUpdated;
        BattleManager.Instance.PlayerDataUpdated += HandlePlayerDataUpdated;

        if (BattleManager.Instance.PlayerData != null)
            RandomizeSelectedMoves();
    }

    private void UnbindPlayerData()
    {
        if (BattleManager.Instance == null)
            return;

        BattleManager.Instance.PlayerDataUpdated -= HandlePlayerDataUpdated;
    }

    private void HandlePlayerDataUpdated()
    {
        RandomizeSelectedMoves();
    }

    private void RandomizeSelectedMoves()
    {
        if (_model == null)
            return;

        if (BattleManager.Instance == null || BattleManager.Instance.PlayerData == null)
        {
            _model.ClearMoves();
            return;
        }

        PokemonData pokemon = BattleManager.Instance.PlayerData;
        MoveDataList moveDataList = GameManager.Instance != null ? GameManager.Instance.MoveDataList : null;
        if (pokemon.MoveNames == null || pokemon.MoveNames.Length == 0 || moveDataList == null)
        {
            _model.ClearMoves();
            return;
        }

        List<MoveData> resolvedMoves = new List<MoveData>();
        for (int i = 0; i < pokemon.MoveNames.Length; i++)
        {
            MoveData move = moveDataList.GetByName(pokemon.MoveNames[i]);
            if (move != null)
                resolvedMoves.Add(move);
        }

        if (resolvedMoves.Count == 0)
        {
            _model.ClearMoves();
            return;
        }

        Shuffle(resolvedMoves);

        int takeCount = Mathf.Min(4, resolvedMoves.Count);
        MoveData[] selected = new MoveData[takeCount];
        for (int i = 0; i < takeCount; i++)
            selected[i] = resolvedMoves[i];

        _model.SetMoves(selected);
    }

    private static void Shuffle(List<MoveData> list)
    {
        for (int i = list.Count - 1; i > 0; i--)
        {
            int j = Random.Range(0, i + 1);
            MoveData temp = list[i];
            list[i] = list[j];
            list[j] = temp;
        }
    }
}
