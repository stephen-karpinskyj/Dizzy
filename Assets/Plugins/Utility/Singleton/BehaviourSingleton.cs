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
                instance = Object.FindObjectOfType<T>();

                if (instance == null)
                {
                    instance = GameObjectUtility.InstantiateComponent<T>();
                }

                Debug.Assert(instance != null, instance);
                Object.DontDestroyOnLoad(instance);
            }

            return instance;
        }
    }

    public static bool Exists
    {
        get { return instance != null; }
    }
}
