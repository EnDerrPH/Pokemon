using TMPro;
using UnityEngine;

public class MoveButtonUI : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private TextMeshProUGUI _text;

    [SerializeField] private MoveData _moveData;

    public MoveData MoveData => _moveData;

    public void SetMove(MoveData moveData)
    {
        _moveData = moveData;

        if (moveData == null)
        {
            SetText(string.Empty);
            return;
        }

        SetText(moveData.MoveName);
    }

    public void SetText(string text)
    {
        if (_text == null)
            return;

        _text.SetText(text ?? string.Empty);
    }
}
