using UnityEngine;

/// <summary>
/// Useful for creating stateless singletons that play nicely during assembly reloads in the editor.
/// OnDisable/OnEnable is called on script compilation and entering play mode.
/// </summary>
/// <remarks>Originated from: http://forum.unity3d.com/threads/scriptablesinglton-t.294436/#post-1944686</remarks>
public abstract class ScriptableService<T> : ScriptableObject where T : ScriptableObject
{
    private static T instance;

    public static T Instance
    {
        get
        {
            ExplicitlyInstantiate();
            return instance;
        }
    }

    protected ScriptableService()
    {
        if (ScriptableService<T>.instance != null)
        {
            Debug.LogError(this.GetType().Name + " already exists. Did you query the singleton in a constructor?");
        }

        ScriptableService<T>.instance = this as T;
    }

    public static bool CheckExists()
    {
        return instance != null;
    }

    public static void ExplicitlyInstantiate()
    {
        if (ScriptableService<T>.instance == null)
        {
            var t = ScriptableObject.CreateInstance<T>();
            t.hideFlags = HideFlags.HideAndDontSave;

            ScriptableService<T>.instance = t;
        }
    }

    public static bool Stop()
    {
        if (CheckExists())
        {
            Object.DestroyImmediate(instance);
            instance = null;

            return true;
        }

        return false;
    }
}