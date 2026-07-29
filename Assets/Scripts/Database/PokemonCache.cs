using System.IO;
using UnityEngine;

public class PokemonCache
{
    public const int CacheVersion = 4;

    private readonly string _rootPath;
    private readonly PokemonDownloader _downloader = new();

    public PokemonCache()
    {
        _rootPath = Path.Combine(Application.persistentDataPath, "pokemon_cache");
    }

    public string RootPath => _rootPath;

    public bool TryLoadManifest(int expectedCount, out PokemonCacheManifest manifest)
    {
        manifest = null;

        string manifestPath = GetManifestPath();
        if (!File.Exists(manifestPath))
            return false;

        try
        {
            string json = File.ReadAllText(manifestPath);
            manifest = JsonUtility.FromJson<PokemonCacheManifest>(json);
        }
        catch
        {
            return false;
        }

        if (manifest == null)
            return false;

        if (manifest.cacheVersion != CacheVersion)
            return false;

        if (manifest.expectedCount != expectedCount)
            return false;

        if (manifest.listItems == null || manifest.listItems.Length != expectedCount)
            return false;

        for (int id = 1; id <= expectedCount; id++)
        {
            if (!File.Exists(GetEntryPath(id)))
                return false;
        }

        return true;
    }

    public void BeginFreshSave()
    {
        if (Directory.Exists(_rootPath))
            Directory.Delete(_rootPath, true);

        Directory.CreateDirectory(_rootPath);
    }

    public void SavePokemon(PokemonBuildResult result)
    {
        if (result?.Data == null)
            return;

        Directory.CreateDirectory(_rootPath);

        PokemonData data = result.Data;
        PokemonCacheEntry entry = new PokemonCacheEntry
        {
            id = data.Id,
            pokemonName = data.PokemonName,
            types = data.Types,
            moveNames = data.MoveNames,
            hp = data.Hp,
            attack = data.Attack,
            defense = data.Defense,
            speed = data.Speed,
            frontAnimDelays = data.FrontAnimDelays,
            backAnimDelays = data.BackAnimDelays,
            hasIconPng = result.IconPngBytes != null && result.IconPngBytes.Length > 0,
            hasFrontGif = result.FrontGifBytes != null && result.FrontGifBytes.Length > 0,
            hasBackGif = result.BackGifBytes != null && result.BackGifBytes.Length > 0
        };

        File.WriteAllText(GetEntryPath(data.Id), JsonUtility.ToJson(entry));

        if (entry.hasIconPng)
            File.WriteAllBytes(GetIconPath(data.Id), result.IconPngBytes);

        if (entry.hasFrontGif)
            File.WriteAllBytes(GetFrontGifPath(data.Id), result.FrontGifBytes);

        if (entry.hasBackGif)
            File.WriteAllBytes(GetBackGifPath(data.Id), result.BackGifBytes);
    }

    public void SaveManifest(int expectedCount, PokemonListItemDTO[] listItems)
    {
        Directory.CreateDirectory(_rootPath);

        PokemonCacheManifest manifest = new PokemonCacheManifest
        {
            cacheVersion = CacheVersion,
            expectedCount = expectedCount,
            listItems = listItems
        };

        File.WriteAllText(GetManifestPath(), JsonUtility.ToJson(manifest));
    }

    public PokemonData LoadPokemon(int id)
    {
        string entryPath = GetEntryPath(id);
        if (!File.Exists(entryPath))
            return null;

        PokemonCacheEntry entry;
        try
        {
            entry = JsonUtility.FromJson<PokemonCacheEntry>(File.ReadAllText(entryPath));
        }
        catch
        {
            return null;
        }

        if (entry == null)
            return null;

        byte[] iconPngBytes = null;
        byte[] frontGifBytes = null;
        byte[] backGifBytes = null;

        string iconPath = GetIconPath(id);
        string frontPath = GetFrontGifPath(id);
        string backPath = GetBackGifPath(id);

        if (entry.hasIconPng && File.Exists(iconPath))
            iconPngBytes = File.ReadAllBytes(iconPath);

        if (entry.hasFrontGif && File.Exists(frontPath))
            frontGifBytes = File.ReadAllBytes(frontPath);

        if (entry.hasBackGif && File.Exists(backPath))
            backGifBytes = File.ReadAllBytes(backPath);

        return _downloader.BuildPokemonFromCache(entry, iconPngBytes, frontGifBytes, backGifBytes);
    }

    private string GetManifestPath() => Path.Combine(_rootPath, "manifest.json");
    private string GetEntryPath(int id) => Path.Combine(_rootPath, $"{id}.json");
    private string GetIconPath(int id) => Path.Combine(_rootPath, $"{id}_icon.png");
    private string GetFrontGifPath(int id) => Path.Combine(_rootPath, $"{id}_front.gif");
    private string GetBackGifPath(int id) => Path.Combine(_rootPath, $"{id}_back.gif");
}
