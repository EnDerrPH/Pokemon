using UnityEngine;
using UnityEngine.Networking;
using Cysharp.Threading.Tasks;

public class PokemonApi
{
    private const string URL = "https://pokeapi.co/api/v2/pokemon/";

    public async UniTask<PokemonDTO> GetPokemon(int id)
    {
        using UnityWebRequest request = UnityWebRequest.Get(URL + id);

        await request.SendWebRequest();

        if (request.result != UnityWebRequest.Result.Success)
        {
            Debug.LogError(request.error);
            return null;
        }

        return JsonUtility.FromJson<PokemonDTO>(request.downloadHandler.text);
    }

public async UniTask<PokemonListDTO> GetPokemonList(int limit = 151)
    {
        using UnityWebRequest request = UnityWebRequest.Get($"{URL}?limit={limit}");

        await request.SendWebRequest();

        if (request.result != UnityWebRequest.Result.Success)
        {
            Debug.LogError(request.error);
            return null;
        }

        return JsonUtility.FromJson<PokemonListDTO>(request.downloadHandler.text);
    }

}

