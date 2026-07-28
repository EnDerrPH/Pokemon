using Cysharp.Threading.Tasks;
using UnityEngine;

public class LoadingBarController : AController<LoadingBarModel>
{
    [Header("Loading")]
    [SerializeField] private int _pokemonLimit = 151;

    private readonly PokemonApi _api = new();
    private readonly PokemonDatabase _database = new();
    private PokemonCache _cache;

    private void OnEnable()
    {
        if (_model == null) return;

        _cache ??= new PokemonCache();

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
        _model.LoadingComplete?.Invoke();
    }
}
