using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Pokemon/Pokemon Data List")]
public class PokemonDataList : ScriptableObject
{
    [SerializeField] private List<PokemonData> _pokemon = new();

    private readonly Dictionary<int, PokemonData> _byId = new();

    public int Count => _pokemon.Count;

    public void Clear()
    {
        _pokemon.Clear();
        _byId.Clear();
    }

    public void Add(PokemonData data)
    {
        if (data == null) return;

        if (_byId.ContainsKey(data.Id))
        {
            int index = _pokemon.FindIndex(p => p != null && p.Id == data.Id);
            if (index >= 0)
                _pokemon[index] = data;

            _byId[data.Id] = data;
            return;
        }

        _pokemon.Add(data);
        _byId[data.Id] = data;
    }

    public PokemonData GetById(int id)
    {
        RebuildLookupIfNeeded();
        return _byId.TryGetValue(id, out PokemonData data) ? data : null;
    }

    public PokemonData GetByIndex(int index)
    {
        if (index < 0 || index >= _pokemon.Count)
            return null;

        return _pokemon[index];
    }

    public IReadOnlyList<PokemonData> GetAll()
    {
        return _pokemon;
    }

    private void RebuildLookupIfNeeded()
    {
        if (_byId.Count == _pokemon.Count)
            return;

        _byId.Clear();
        for (int i = 0; i < _pokemon.Count; i++)
        {
            PokemonData data = _pokemon[i];
            if (data == null) continue;
            _byId[data.Id] = data;
        }
    }
}
