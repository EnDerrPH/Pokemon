using System.Collections.Generic;
using Cysharp.Threading.Tasks;

public class PokemonDatabase
{
    private readonly Dictionary<int, PokemonBuildResult> _cache = new();
    private readonly PokemonApi _api = new();
    private readonly PokemonDownloader _downloader = new();

    public async UniTask<PokemonBuildResult> GetPokemon(int id)
    {
        if (_cache.ContainsKey(id))
            return _cache[id];

        PokemonDTO dto = await _api.GetPokemon(id);
        if (dto == null)
            return null;

        PokemonBuildResult result = await _downloader.BuildPokemon(dto);
        _cache[id] = result;

        return result;
    }
}
