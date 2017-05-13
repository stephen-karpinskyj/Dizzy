using UnityEngine;

public abstract class ScriptableAsset<T> : ScriptableSingleton<T> where T : ScriptableObject
{
    #region Constants


    private const string PathToResources = "Assets/Resources/";
    private const string AssetExtension = "asset";


    #endregion


    #region Protected


    protected static T LoadFromResources(string assetName)
    {
        T asset = Resources.Load(assetName) as T;

        Debug.Assert(asset != null, asset);

        return asset;
    }


    #endregion


    #if UNITY_EDITOR
    /// <remarks>Editor-only.</remarks>
    protected static void EDITOR_CreateOrResetAsset()
    {
        const string ResourcesFolder = PathToResources;
        var assetPath = ResourcesFolder;

        if (!System.IO.Directory.Exists(ResourcesFolder))
        {
            System.IO.Directory.CreateDirectory(ResourcesFolder);
        }

        if (!System.IO.Directory.Exists(assetPath))
        {
            System.IO.Directory.CreateDirectory(assetPath);
        }

        assetPath = System.IO.Path.Combine(assetPath, typeof(T).Name);
        assetPath = System.IO.Path.ChangeExtension(assetPath, AssetExtension);

        UnityEditor.AssetDatabase.DeleteAsset(assetPath);

        var obj = ScriptableObject.CreateInstance<T>();

        UnityEditor.AssetDatabase.CreateAsset(obj, assetPath);
        UnityEditor.AssetDatabase.SaveAssets();
        UnityEditor.Selection.activeObject = obj;

        Debug.LogFormat(obj, "Created/reset game data asset stored under \"{0}\"", assetPath);
    }
    #endif
}
