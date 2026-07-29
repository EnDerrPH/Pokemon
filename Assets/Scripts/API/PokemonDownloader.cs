using UnityEngine;
using UnityEngine.Networking;
using Cysharp.Threading.Tasks;

public class PokemonDownloader
{
    private const string FrontAnimUrlFormat =
        "https://play.pokemonshowdown.com/sprites/ani/{0}.gif";
    private const string BackAnimUrlFormat =
        "https://play.pokemonshowdown.com/sprites/ani-back/{0}.gif";

    public async UniTask<byte[]> DownloadBytes(string url)
    {
        if (string.IsNullOrEmpty(url))
            return null;

        using UnityWebRequest request = UnityWebRequest.Get(url);
        UnityWebRequestAsyncOperation operation = request.SendWebRequest();

        while (!operation.isDone)
            await UniTask.Yield();

        if (request.result != UnityWebRequest.Result.Success)
        {
            Debug.LogWarning($"Failed to download: {url} ({request.responseCode} {request.error})");
            return null;
        }

        return request.downloadHandler.data;
    }

    public async UniTask<PokemonBuildResult> BuildPokemon(PokemonDTO dto)
    {
        PokemonData data = ScriptableObject.CreateInstance<PokemonData>();

        string[] types = new string[dto.types.Length];
        for (int i = 0; i < dto.types.Length; i++)
            types[i] = dto.types[i].type.name;

        string[] moveNames = Gen1MoveHelper.ExtractMoveNames(dto);

        int hp = 0;
        int attack = 0;
        int defense = 0;
        int speed = 0;

        foreach (var stat in dto.stats)
        {
            switch (stat.stat.name)
            {
                case "hp":
                    hp = stat.base_stat;
                    break;
                case "attack":
                    attack = stat.base_stat;
                    break;
                case "defense":
                    defense = stat.base_stat;
                    break;
                case "speed":
                    speed = stat.base_stat;
                    break;
            }
        }

        byte[] iconPngBytes = await DownloadBytes(dto.sprites?.front_default);
        Sprite pokemonIcon = CreateSpriteFromImageBytes(iconPngBytes);

        string showdownName = ToShowdownName(dto.name);
        string frontUrl = string.Format(FrontAnimUrlFormat, showdownName);
        string backUrl = string.Format(BackAnimUrlFormat, showdownName);

        byte[] frontGifBytes = await DownloadBytes(frontUrl);
        byte[] backGifBytes = await DownloadBytes(backUrl);

        GifDecodeResult frontAnim = frontGifBytes != null ? GifDecoder.Decode(frontGifBytes) : null;
        GifDecodeResult backAnim = backGifBytes != null ? GifDecoder.Decode(backGifBytes) : null;

        Sprite frontSprite = GetFirstFrame(frontAnim);
        Sprite backSprite = GetFirstFrame(backAnim);

        data.Initialize(
            dto.id,
            dto.name,
            pokemonIcon,
            frontSprite,
            backSprite,
            frontAnim?.Frames,
            backAnim?.Frames,
            frontAnim?.Delays,
            backAnim?.Delays,
            types,
            moveNames,
            hp,
            attack,
            defense,
            speed
        );

        return new PokemonBuildResult
        {
            Data = data,
            IconPngBytes = iconPngBytes,
            FrontGifBytes = frontGifBytes,
            BackGifBytes = backGifBytes
        };
    }

    public PokemonData BuildPokemonFromCache(
        PokemonCacheEntry entry,
        byte[] iconPngBytes,
        byte[] frontGifBytes,
        byte[] backGifBytes)
    {
        if (entry == null)
            return null;

        Sprite pokemonIcon = CreateSpriteFromImageBytes(iconPngBytes);
        GifDecodeResult frontAnim = frontGifBytes != null ? GifDecoder.Decode(frontGifBytes) : null;
        GifDecodeResult backAnim = backGifBytes != null ? GifDecoder.Decode(backGifBytes) : null;

        PokemonData data = ScriptableObject.CreateInstance<PokemonData>();
        data.Initialize(
            entry.id,
            entry.pokemonName,
            pokemonIcon,
            GetFirstFrame(frontAnim),
            GetFirstFrame(backAnim),
            frontAnim?.Frames,
            backAnim?.Frames,
            frontAnim?.Delays ?? entry.frontAnimDelays,
            backAnim?.Delays ?? entry.backAnimDelays,
            entry.types,
            entry.moveNames,
            entry.hp,
            entry.attack,
            entry.defense,
            entry.speed
        );

        return data;
    }

    public MoveData BuildMove(MoveDTO dto)
    {
        if (dto == null)
            return null;

        MoveData data = ScriptableObject.CreateInstance<MoveData>();
        data.Initialize(
            dto.id,
            dto.name,
            dto.power,
            dto.pp,
            dto.accuracy,
            dto.priority,
            dto.type != null ? dto.type.name : null,
            dto.damage_class != null ? dto.damage_class.name : null
        );

        return data;
    }

    public MoveData BuildMoveFromCache(MoveCacheEntry entry)
    {
        if (entry == null)
            return null;

        MoveData data = ScriptableObject.CreateInstance<MoveData>();
        data.Initialize(
            entry.id,
            entry.moveName,
            entry.power,
            entry.pp,
            entry.accuracy,
            entry.priority,
            entry.type,
            entry.damageClass
        );

        return data;
    }

    private static Sprite CreateSpriteFromImageBytes(byte[] bytes)
    {
        if (bytes == null || bytes.Length == 0)
            return null;

        Texture2D texture = new Texture2D(2, 2, TextureFormat.RGBA32, false);
        if (!texture.LoadImage(bytes))
        {
            UnityEngine.Object.Destroy(texture);
            return null;
        }

        texture.filterMode = FilterMode.Point;

        return Sprite.Create(
            texture,
            new Rect(0, 0, texture.width, texture.height),
            new Vector2(0.5f, 0f)
        );
    }

    private static Sprite GetFirstFrame(GifDecodeResult anim)
    {
        if (anim?.Frames == null || anim.Frames.Length == 0)
            return null;

        return anim.Frames[0];
    }

    private static string ToShowdownName(string pokemonName)
    {
        if (string.IsNullOrEmpty(pokemonName))
            return pokemonName;

        switch (pokemonName)
        {
            case "nidoran-f":
                return "nidoranf";
            case "nidoran-m":
                return "nidoranm";
            case "mr-mime":
                return "mrmime";
            default:
                return pokemonName;
        }
    }
}
