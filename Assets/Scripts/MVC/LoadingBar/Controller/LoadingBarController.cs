using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;

public class LoadingBarController : AController<LoadingBarModel>
{
    [Header("Loading")]
    [SerializeField] private int _pokemonLimit = 151;

    private readonly PokemonApi _api = new();
    private readonly PokemonDatabase _database = new();
    private readonly PokemonDownloader _downloader = new();
    private PokemonCache _cache;
    private MoveCache _moveCache;

    private void OnEnable()
    {
        if (_model == null) return;

        _cache ??= new PokemonCache();
        _moveCache ??= new MoveCache();

        _model.SubmitStartLoading += HandleStartLoading;
        _model.SubmitStartLoading?.Invoke();
    }

    private void OnDisable()
    {
        if (_model == null) return;

        _model.SubmitStartLoading -= HandleStartLoading;
    }

    private void HandleStartLoading()
    {
        FetchDataAsync().Forget();
    }

    private async UniTaskVoid FetchDataAsync()
    {
        _cache ??= new PokemonCache();
        _moveCache ??= new MoveCache();
        _model.SetProgress(0, 0);

        if (_cache.TryLoadManifest(_pokemonLimit, out PokemonCacheManifest manifest))
        {
            await LoadFromCacheAsync(manifest);
            return;
        }

        await FetchAndCacheAsync();
    }

    private async UniTask LoadFromCacheAsync(PokemonCacheManifest manifest)
    {
        int totalCount = manifest.expectedCount;
        _model.SetProgress(0, totalCount);

        PokemonDataList pokemonDataList = GameManager.Instance.PokemonDataList;
        pokemonDataList.Clear();

        if (GlobalModelLocator.Instance != null)
        {
            PokedexModel pokedexModel = GlobalModelLocator.Instance.GetModel<PokedexModel>();
            pokedexModel.SetPokemonList(manifest.listItems);
        }

        for (int i = 0; i < totalCount; i++)
        {
            int pokemonId = i + 1;
            PokemonData data = _cache.LoadPokemon(pokemonId);
            if (data != null)
                pokemonDataList.Add(data);

            _model.SetProgress(i + 1, totalCount);
            await UniTask.Yield();
        }

        SetDefaultSelectedPokemon(pokemonDataList);
        await LoadMovesAsync(pokemonDataList);
        _model.LoadingComplete?.Invoke();
    }

    private async UniTask FetchAndCacheAsync()
    {
        PokemonListDTO list = await _api.GetPokemonList(_pokemonLimit);
        if (list?.results == null || list.results.Length == 0)
        {
            Debug.LogError("Failed to load Pokemon list");
            return;
        }

        int totalCount = list.results.Length;
        _model.SetProgress(0, totalCount);

        PokemonDataList pokemonDataList = GameManager.Instance.PokemonDataList;
        pokemonDataList.Clear();

        if (GlobalModelLocator.Instance != null)
        {
            PokedexModel pokedexModel = GlobalModelLocator.Instance.GetModel<PokedexModel>();
            pokedexModel.SetPokemonList(list.results);
        }

        _cache.BeginFreshSave();

        for (int i = 0; i < totalCount; i++)
        {
            int pokemonId = i + 1;
            PokemonBuildResult result = await _database.GetPokemon(pokemonId);
            if (result?.Data != null)
            {
                pokemonDataList.Add(result.Data);
                _cache.SavePokemon(result);
            }

            _model.SetProgress(i + 1, totalCount);
        }

        _cache.SaveManifest(totalCount, list.results);
        SetDefaultSelectedPokemon(pokemonDataList);
        await LoadMovesAsync(pokemonDataList);
        _model.LoadingComplete?.Invoke();
    }

    private async UniTask LoadMovesAsync(PokemonDataList pokemonDataList)
    {
        MoveDataList moveDataList = GameManager.Instance.MoveDataList;
        moveDataList.Clear();

        if (_moveCache.TryLoad(out MoveCacheEntry[] cachedMoves))
        {
            for (int i = 0; i < cachedMoves.Length; i++)
            {
                MoveData move = _downloader.BuildMoveFromCache(cachedMoves[i]);
                if (move != null)
                    moveDataList.Add(move);
            }

            return;
        }

        HashSet<string> uniqueMoveNames = CollectUniqueMoveNames(pokemonDataList);
        if (uniqueMoveNames.Count == 0)
            return;

        string[] moveNames = new string[uniqueMoveNames.Count];
        uniqueMoveNames.CopyTo(moveNames);

        int loadedPokemon = pokemonDataList.Count;
        int total = loadedPokemon + moveNames.Length;
        _model.SetProgress(loadedPokemon, total);

        for (int i = 0; i < moveNames.Length; i++)
        {
            MoveDTO dto = await _api.GetMove(moveNames[i]);
            if (dto != null)
            {
                MoveData move = _downloader.BuildMove(dto);
                if (move != null)
                    moveDataList.Add(move);
            }

            _model.SetProgress(loadedPokemon + i + 1, total);
        }

        _moveCache.Save(moveDataList);
    }

    private static HashSet<string> CollectUniqueMoveNames(PokemonDataList pokemonDataList)
    {
        HashSet<string> names = new HashSet<string>();
        if (pokemonDataList == null)
            return names;

        IReadOnlyList<PokemonData> all = pokemonDataList.GetAll();
        for (int i = 0; i < all.Count; i++)
        {
            PokemonData pokemon = all[i];
            if (pokemon?.MoveNames == null)
                continue;

            for (int j = 0; j < pokemon.MoveNames.Length; j++)
            {
                string moveName = pokemon.MoveNames[j];
                if (!string.IsNullOrEmpty(moveName))
                    names.Add(moveName);
            }
        }

        return names;
    }

    private static void SetDefaultSelectedPokemon(PokemonDataList pokemonDataList)
    {
        if (GameManager.Instance == null || pokemonDataList == null || pokemonDataList.Count == 0)
            return;

        GameManager.Instance.SetSelectedPokemonData(pokemonDataList.GetByIndex(0));
    }
}
