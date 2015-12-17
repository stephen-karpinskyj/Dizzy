using UnityEngine;

public abstract class BehaviourSingleton<T> : MonoBehaviour where T : MonoBehaviour
{
    protected static T instance;

    public static T Instance
    {
        get
        {
            if (!instance)
            {
                instance = GameObjectUtility.InstantiateBehaviour<T>();
                Debug.Assert(instance != null, instance);
            }

            return instance;
        }
    }

    public static bool Exists
    {
        get { return instance != null; }
    }

    protected virtual void Awake()
    {
        Object.DontDestroyOnLoad(this);
    }
}
