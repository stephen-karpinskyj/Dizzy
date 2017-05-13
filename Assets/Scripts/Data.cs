using System.Collections.Generic;
using UnityEngine;

public class Data : ScriptableAsset<Data>
{
    #region Inspector


    [Header("Prefabs")]
    
    [SerializeField]
    private JunkController junk;
    
    [SerializeField]
    private MineController mine;
    
    [SerializeField]
    private LevelData defaultLevel;
    
    [SerializeField]
    private List<LevelData> levels;


    #endregion


    #region Properties


    public JunkController JunkPrefab { get { return this.junk; } }
    
    public MineController MinePrefab { get { return this.mine; } }

    public LevelData GetDefaultLevel()
    {
        return this.defaultLevel;
    }

    public LevelData GetNextLevel(LevelData level, bool forward)
    {
        var index = this.levels.FindIndex(l => l.Id == level.Id) + (forward ? 1 : -1);
        
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
        ScriptableAsset<Data>.EDITOR_CreateOrResetAsset();
    }


    #endregion

    #endif
}
