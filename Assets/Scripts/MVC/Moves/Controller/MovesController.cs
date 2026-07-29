using System.Collections.Generic;
using UnityEngine;

public class MovesController : AController<MovesModel>
{
    private void OnEnable()
    {
        BindSelectedPokemon();
    }

    private void Start()
    {
        BindSelectedPokemon();
    }

    private void OnDisable()
    {
        UnbindSelectedPokemon();
    }

    private void BindSelectedPokemon()
    {
        if (GameManager.Instance == null)
            return;

        GameManager.Instance.SelectedPokemonUpdated -= HandleSelectedPokemonUpdated;
        GameManager.Instance.SelectedPokemonUpdated += HandleSelectedPokemonUpdated;

        if (GameManager.Instance.SelectedPokemonData != null)
            RandomizeSelectedMoves();
    }

    private void UnbindSelectedPokemon()
    {
        if (GameManager.Instance == null)
            return;

        GameManager.Instance.SelectedPokemonUpdated -= HandleSelectedPokemonUpdated;
    }

    private void HandleSelectedPokemonUpdated()
    {
        RandomizeSelectedMoves();
    }

    private void RandomizeSelectedMoves()
    {
        if (_model == null)
            return;

        if (GameManager.Instance == null || GameManager.Instance.SelectedPokemonData == null)
        {
            _model.ClearMoves();
            return;
        }

        PokemonData pokemon = GameManager.Instance.SelectedPokemonData;
        MoveDataList moveDataList = GameManager.Instance.MoveDataList;
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
