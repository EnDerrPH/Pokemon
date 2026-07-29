using UnityEngine;

public class MoveListUI : AView<MovesModel>
{
    [Header("Buttons")]
    [SerializeField] private MoveButtonUI[] _moveButtons = new MoveButtonUI[4];

    private void OnEnable()
    {
        if (_model == null)
            return;

        _model.MovesUpdated += HandleMovesUpdated;
        HandleMovesUpdated();
    }

    private void OnDisable()
    {
        if (_model == null)
            return;

        _model.MovesUpdated -= HandleMovesUpdated;
    }

    private void HandleMovesUpdated()
    {
        if (_model == null || _moveButtons == null)
            return;

        MoveData[] moves = _model.SelectedMoves;

        for (int i = 0; i < _moveButtons.Length; i++)
        {
            MoveButtonUI button = _moveButtons[i];
            if (button == null)
                continue;

            MoveData move = moves != null && i < moves.Length ? moves[i] : null;
            button.SetMove(move);
        }
    }
}
