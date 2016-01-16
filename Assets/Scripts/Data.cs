using UnityEngine;

public class Data : DataAsset<Data>
{
    #region Inspector


    [Header("Prefabs")]
    
    [SerializeField]
    private JunkController junk;


    #endregion


    #region Properties


    public JunkController JunkPrefab { get { return this.junk; } }


    #endregion


    #if UNITY_EDITOR

    #region Editor


    [UnityEditor.MenuItem("Dizzy/Create or Reset Data")]
    private static void CreateResetData()
    {
        DataAsset<Data>.CreateOrResetAsset(AssetName);
    }


    #endregion

    #endif
}
