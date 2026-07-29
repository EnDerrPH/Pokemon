using System;
using System.IO;
using UnityEngine;

[Serializable]
public class MoveCacheFile
{
    public int cacheVersion;
    public MoveCacheEntry[] moves;
}

[Serializable]
public class MoveCacheEntry
{
    public int id;
    public string moveName;
    public int power;
    public int pp;
    public int accuracy;
    public int priority;
    public string type;
    public string damageClass;
}

public class MoveCache
{
    public const int CacheVersion = 1;

    private readonly string _rootPath;

    public MoveCache()
    {
        _rootPath = Path.Combine(Application.persistentDataPath, "move_cache");
    }

    public bool TryLoad(out MoveCacheEntry[] entries)
    {
        entries = null;
        string path = GetFilePath();
        if (!File.Exists(path))
            return false;

        try
        {
            MoveCacheFile file = JsonUtility.FromJson<MoveCacheFile>(File.ReadAllText(path));
            if (file == null || file.cacheVersion != CacheVersion || file.moves == null || file.moves.Length == 0)
                return false;

            entries = file.moves;
            return true;
        }
        catch
        {
            return false;
        }
    }

    public void Save(MoveDataList moveDataList)
    {
        if (moveDataList == null || moveDataList.Count == 0)
            return;

        Directory.CreateDirectory(_rootPath);

        MoveCacheEntry[] entries = new MoveCacheEntry[moveDataList.Count];
        for (int i = 0; i < moveDataList.Count; i++)
        {
            MoveData data = moveDataList.GetByIndex(i);
            if (data == null)
                continue;

            entries[i] = new MoveCacheEntry
            {
                id = data.Id,
                moveName = data.MoveName,
                power = data.Power,
                pp = data.Pp,
                accuracy = data.Accuracy,
                priority = data.Priority,
                type = data.Type,
                damageClass = data.DamageClass
            };
        }

        MoveCacheFile file = new MoveCacheFile
        {
            cacheVersion = CacheVersion,
            moves = entries
        };

        File.WriteAllText(GetFilePath(), JsonUtility.ToJson(file));
    }

    public void Clear()
    {
        if (Directory.Exists(_rootPath))
            Directory.Delete(_rootPath, true);
    }

    private string GetFilePath() => Path.Combine(_rootPath, "moves.json");
}
