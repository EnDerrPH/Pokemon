using UnityEngine;
using UnityEngine.Networking;
using Cysharp.Threading.Tasks;

public class PokemonApi
{
    private const string PokemonUrl = "https://pokeapi.co/api/v2/pokemon/";
    private const string MoveUrl = "https://pokeapi.co/api/v2/move/";

    public async UniTask<PokemonDTO> GetPokemon(int id)
    {
        string json = await GetJson(PokemonUrl + id);
        if (string.IsNullOrEmpty(json))
            return null;

        return JsonUtility.FromJson<PokemonDTO>(json);
    }

    public async UniTask<PokemonListDTO> GetPokemonList(int limit = 151)
    {
        string json = await GetJson($"{PokemonUrl}?limit={limit}");
        if (string.IsNullOrEmpty(json))
            return null;

        return JsonUtility.FromJson<PokemonListDTO>(json);
    }

    public async UniTask<MoveDTO> GetMove(string moveNameOrId)
    {
        if (string.IsNullOrEmpty(moveNameOrId))
            return null;

        string json = await GetJson(MoveUrl + moveNameOrId);
        if (string.IsNullOrEmpty(json))
            return null;

        return JsonUtility.FromJson<MoveDTO>(json);
    }

    private static async UniTask<string> GetJson(string url)
    {
        using UnityWebRequest request = UnityWebRequest.Get(url);
        UnityWebRequestAsyncOperation operation = request.SendWebRequest();

        while (!operation.isDone)
            await UniTask.Yield();

        if (request.result != UnityWebRequest.Result.Success)
        {
            Debug.LogWarning($"API request failed: {url} ({request.responseCode} {request.error})");
            return null;
        }

        return request.downloadHandler.text;
    }
}
