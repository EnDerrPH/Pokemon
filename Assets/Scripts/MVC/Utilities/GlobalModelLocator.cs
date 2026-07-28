using System.Collections.Generic;
using UnityEngine;

public class GlobalModelLocator : AMonoSingleton<GlobalModelLocator>
{
    protected override bool DontDestroyOnLoad => true;

    private readonly Dictionary<string, object> _models = new();

    public T GetModel<T>(string id = "") where T : new()
    {
        string key = GetKey<T>(id);

        if (!_models.TryGetValue(key, out object foundModel))
        {
            T createdModel = new T();
            _models[key] = createdModel;
            return createdModel;
        }

        return (T)foundModel;
    }

    public void RegisterModel<T>(T model, string id = "")
    {
        _models[GetKey<T>(id)] = model;
    }

    private static string GetKey<T>(string id)
    {
        return $"{typeof(T).FullName}:{id}";
    }
}
