using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Pokemon/Move Data List")]
public class MoveDataList : ScriptableObject
{
    [SerializeField] private List<MoveData> _moves = new();

    private readonly Dictionary<int, MoveData> _byId = new();
    private readonly Dictionary<string, MoveData> _byName = new();

    public int Count => _moves.Count;

    public void Clear()
    {
        _moves.Clear();
        _byId.Clear();
        _byName.Clear();
    }

    public void Add(MoveData data)
    {
        if (data == null)
            return;

        if (_byId.ContainsKey(data.Id))
        {
            int index = _moves.FindIndex(m => m != null && m.Id == data.Id);
            if (index >= 0)
                _moves[index] = data;

            _byId[data.Id] = data;
            _byName[data.MoveName] = data;
            return;
        }

        _moves.Add(data);
        _byId[data.Id] = data;
        if (!string.IsNullOrEmpty(data.MoveName))
            _byName[data.MoveName] = data;
    }

    public MoveData GetById(int id)
    {
        RebuildLookupIfNeeded();
        return _byId.TryGetValue(id, out MoveData data) ? data : null;
    }

    public MoveData GetByName(string moveName)
    {
        RebuildLookupIfNeeded();
        if (string.IsNullOrEmpty(moveName))
            return null;

        return _byName.TryGetValue(moveName, out MoveData data) ? data : null;
    }

    public MoveData GetByIndex(int index)
    {
        if (index < 0 || index >= _moves.Count)
            return null;

        return _moves[index];
    }

    public IReadOnlyList<MoveData> GetAll()
    {
        return _moves;
    }

    private void RebuildLookupIfNeeded()
    {
        if (_byId.Count == _moves.Count)
            return;

        _byId.Clear();
        _byName.Clear();
        for (int i = 0; i < _moves.Count; i++)
        {
            MoveData data = _moves[i];
            if (data == null)
                continue;

            _byId[data.Id] = data;
            if (!string.IsNullOrEmpty(data.MoveName))
                _byName[data.MoveName] = data;
        }
    }
}
