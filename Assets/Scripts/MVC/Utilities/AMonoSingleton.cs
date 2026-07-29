using UnityEngine;

public abstract class AMonoSingleton<T> : MonoBehaviour where T : MonoBehaviour
{
    protected abstract bool DontDestroyOnLoad { get; }

    private static T _instance;

    public static T Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindAnyObjectByType<T>();

                if (_instance == null)
                {
                    GameObject singletonObject = new GameObject(typeof(T).Name);
                    _instance = singletonObject.AddComponent<T>();
                    (_instance as AMonoSingleton<T>)?.OnSingletonInstantiated();
                }
            }

            return _instance;
        }
    }

    protected virtual void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(gameObject);
            return;
        }

        _instance = this as T;

        if (DontDestroyOnLoad)
        {
            transform.SetParent(null);
            DontDestroyOnLoad(gameObject);
        }
    }

    protected virtual void OnSingletonInstantiated()
    {
    }
}

