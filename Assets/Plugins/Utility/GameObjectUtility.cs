using System.Collections.Generic;
using UnityEngine;

public static class GameObjectUtility
{
    #region Instantiate


    public static GameObject InstantiateGameObject(string name, Transform parent = null, bool resetTransformAfterParenting = true)
    {
        var go = new GameObject(name);

        if (parent)
        {
            if (resetTransformAfterParenting)
            {
                go.transform.parent = parent;
                ResetLocalTransform(go.transform, true);
            }
            else
            {
                go.transform.SetParent(parent, false);
            }
        }

        return go;
    }

    public static T InstantiateComponent<T>(Transform parent = null) where T : Component
    {
        var go = InstantiateGameObject(typeof(T).Name, parent);

        return go.AddComponent<T>();
    }

    public static T InstantiatePrefab<T>(T prefab, Transform parent = null, bool resetTransformAfterParenting = true) where T : Component
    {
        Debug.Assert(prefab != null, prefab);

        var t = Object.Instantiate(prefab);

        Debug.Assert(t != null, prefab);

        if (parent)
        {
            if (resetTransformAfterParenting)
            {
                t.transform.parent = parent;
                ResetLocalTransform(t.transform, true);
            }
            else
            {
                t.transform.SetParent(parent, false);
            }
        }
        
        return t;
    }


    #endregion


    #region Transform


    public static void ResetLocalTransform(Transform t, bool includeScale)
    {
        Debug.Assert(t, t);

        t.localPosition = Vector3.zero;
        t.localRotation = Quaternion.identity;
        
        if (includeScale)
        {
            t.localScale = Vector3.one;
        }
    }

    public static bool IsAncestorOf(this Transform t, Transform child)
    {
        for (var curr = child; curr != null; curr = curr.parent)
        {
            if (curr == t)
            {
                return true;
            }
        }

        return false;
    }

    public static List<Transform> GetDirectDescendents(this Transform t)
    {
        var children = new List<Transform>();

        for (var i = 0; i < t.childCount; i++)
        {
            children.Add(t.GetChild(i));
        }

        return children;
    }

    public static void SetLayer(this Transform t, int layer) 
    {
        t.gameObject.layer = layer;

        foreach (Transform child in t)
        {
            child.SetLayer(layer);
        }
    }

    #endregion


    #region RectTransform


    private static readonly Vector3[] cornersA = new Vector3[4];
    private static readonly Vector3[] cornersB = new Vector3[4];
    private static Bounds boundsA;
    private static Bounds boundsB;

    /// <remarks>Don't assume this is efficient.</remarks>
    public static bool IsOverlapping(this RectTransform a, RectTransform b)
    {
        Vector3 min;
        Vector3 max;

        a.GetWorldCorners(cornersA);
        min = Vector3.Min(cornersA[0], cornersA[1]);
        max = Vector3.Max(cornersA[0], cornersA[1]);
        for (var i = 2; i < cornersA.Length; i++)
        {
            min = Vector3.Min(min, cornersA[i]);
            max = Vector3.Max(max, cornersA[i]);
        }
        boundsA.SetMinMax(min, max);

        b.GetWorldCorners(cornersB);
        min = Vector3.Min(cornersB[0], cornersB[1]);
        max = Vector3.Max(cornersB[0], cornersB[1]);
        for (var i = 2; i < cornersB.Length; i++)
        {
            min = Vector3.Min(min, cornersB[i]);
            max = Vector3.Max(max, cornersB[i]);
        }
        boundsB.SetMinMax(min, max);

        return boundsA.Intersects(boundsB);
    }


    #endregion


    #region Find

    public static T FindFirstInCurrentScene<T>(GameObject go) where T : Component
    {
        Debug.Assert(go != null);

        var rootGOs = go.scene.GetRootGameObjects();

        foreach (var g in rootGOs)
        {
            var screen = g.GetComponentInChildren<T>(true);

            if (screen != null)
            {
                return screen;
            }
        }

        return null;
    }

    #endregion


    #region Colliders

    /// <summary>
    ///   This method adjust the bounds of a given BoxCollider to the encapsulated bounds of all the renderers in 
    ///   the given Game Object
    /// </summary>
    /// <param name="collider"></param>
    /// <param name="ParentGameObject"></param>
    public static void Fit_BoxCollider_onGameObject(BoxCollider collider, GameObject ParentGameObject)
    {

        bool hasBounds = false;
        Bounds bounds = new Bounds(Vector3.zero, Vector3.zero);
        Renderer[] childRenderers = ParentGameObject.transform.GetComponentsInChildren<Renderer>();

        foreach (var childRenderer in childRenderers)
        {
            if (childRenderer != null)
            {
                if (hasBounds)
                {
                    bounds.Encapsulate(childRenderer.bounds);
                }
                else
                {
                    bounds = childRenderer.bounds;
                    hasBounds = true;
                }
            }
        }

        collider.center = bounds.center - ParentGameObject.transform.position;
        collider.size = bounds.size;
    }

    #endregion

}
