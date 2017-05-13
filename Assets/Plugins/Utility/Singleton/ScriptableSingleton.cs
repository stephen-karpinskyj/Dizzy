using UnityEngine;

public abstract class ScriptableSingleton<T> : ScriptableObject where T : ScriptableObject
{
    private static T instance;

    public static T Instance
    {
        get
        {
            if (!instance)
            {
                instance = Resources.Load<T>(typeof(T).Name);
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
