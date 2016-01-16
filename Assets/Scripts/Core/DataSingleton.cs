using UnityEngine;

public abstract class DataSingleton<T> : ScriptableObject where T : ScriptableObject
{
    private static T instance;

    protected static string AssetName { get { return typeof(T).Name; } }

    public static T Instance
    {
        get
        {
            if (!instance)
            {
                instance = Resources.Load<T>(AssetName);
                Debug.Assert(instance != null, instance);
            }

            return instance;
        }
    }

    public static bool Exists
    {
        get { return instance != null; }
    }
}
