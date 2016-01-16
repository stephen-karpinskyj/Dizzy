using UnityEngine;

public abstract class DataAsset<T> : DataSingleton<T> where T : ScriptableObject
{
    #region Constants


    private const string PathToResources = "Assets/Resources/";
    private const string AssetExtension = ".asset";


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

    #region Editor


    /// <remarks>Editor-only.</remarks>
    protected static void CreateOrResetAsset(string assetName)
    {
        const string ResourcesFolder = PathToResources;
        string assetPath = ResourcesFolder + assetName + AssetExtension;

        if (!System.IO.Directory.Exists(ResourcesFolder))
            System.IO.Directory.CreateDirectory(ResourcesFolder);

        UnityEditor.AssetDatabase.DeleteAsset(assetPath);

        T obj = ScriptableObject.CreateInstance<T>();

        UnityEditor.AssetDatabase.CreateAsset(obj, assetPath);
        UnityEditor.AssetDatabase.SaveAssets();
        UnityEditor.Selection.activeObject = obj;

        Debug.LogFormat(obj, "Created/reset game data asset stored under \"{0}\"", assetPath);
    }


    #endregion

    #endif
}
