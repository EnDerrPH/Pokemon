using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using Cysharp.Threading.Tasks;

public class MoveButtonUI : MonoBehaviour, IPointerClickHandler
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

    public void OnPointerClick(PointerEventData eventData)
    {
        if (_moveData == null || BattleFlowManager.Instance == null)
            return;

        BattleFlowManager.Instance.SubmitPlayerMove(_moveData).Forget();
    }
}
