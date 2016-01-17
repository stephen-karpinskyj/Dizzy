using System.Collections.Generic;
using UnityEngine;

public class Data : DataAsset<Data>
{
    #region Inspector


    [Header("Prefabs")]
    
    [SerializeField]
    private JunkController junk;
    
    [SerializeField]
    private List<LevelData> levels;


    #endregion


    #region Properties


    public JunkController JunkPrefab { get { return this.junk; } }

    public LevelData GetDefaultLevel()
    {
        return this.levels[0];
    }

    public LevelData GetNextLevel(LevelData level, bool forward)
    {
        var index = this.levels.IndexOf(level) + (forward ? 1 : -1);
        
        if (index < 0)
        {
            index = this.levels.Count - 1;
        }
        else if (index >= this.levels.Count)
        {
            index = 0;
        }
        
        return this.levels[index];
    }
    
    public IEnumerable<LevelData> Levels
    {
        get { return this.levels; }
    }
    

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
