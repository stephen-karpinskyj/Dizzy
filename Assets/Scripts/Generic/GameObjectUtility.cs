using UnityEngine;

public static class GameObjectUtility
{
    #region Instantiate


    public static GameObject InstantiateGameObject(string name, Transform parent = null)
    {
        var go = new GameObject(name);

        if (parent)
        {
            go.transform.parent = parent;
        }

        return go;
    }

    public static T InstantiateBehaviour<T>(Transform parent = null) where T : MonoBehaviour
    {
        var go = new GameObject(typeof(T).Name);

        if (parent)
        {
            go.transform.parent = parent;
        }

        return go.AddComponent<T>();
    }

    public static T InstantiatePrefab<T>(T prefab, Transform parent = null) where T : MonoBehaviour
    {
        Debug.Assert(prefab != null, prefab);

        var t = Object.Instantiate(prefab);

        Debug.Assert(t != null, prefab);

        if (parent)
        {
            t.transform.parent = parent;
        }
        
        return t;
    }


    #endregion


    #region Show


    public static void ShowGameObject(GameObject go, bool show)
    {
        Debug.Assert(go, go);

        go.SetActive(show);
    }

    public static void ToggleGameObject(GameObject go)
    {
        Debug.Assert(go, go);

        go.SetActive(!go.activeSelf);
    }


    #endregion


    #region Transform


    public static void ResetLocalTransform(Transform t, bool includeScale)
    {
        if (!t)
        {
            return;
        }

        t.localPosition = Vector3.zero;
        t.localRotation = Quaternion.identity;
        
        if (includeScale)
        {
            t.localScale = Vector3.one;
        }
    }


    #endregion
}
