using System;

public class MovesModel : AModel
{
    public MoveData[] SelectedMoves { get; private set; } = new MoveData[4];

    public Action MovesUpdated;

    public void SetMoves(MoveData[] moves)
    {
        SelectedMoves = new MoveData[4];

        if (moves != null)
        {
            int count = Math.Min(moves.Length, 4);
            for (int i = 0; i < count; i++)
                SelectedMoves[i] = moves[i];
        }

        MovesUpdated?.Invoke();
    }

    public void ClearMoves()
    {
        SetMoves(null);
    }
}
