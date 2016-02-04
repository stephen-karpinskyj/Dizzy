using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Manages data and all saved state.
/// </summary>
public class StateManager : BehaviourSingleton<StateManager>
{
    #region Constants
    
    
    private const int DefaultJunkCount = 0;
    private const int MinJunkCount = DefaultJunkCount;
    private const int MaxJunkCount = 99999999;


    #endregion


    #region Settings


    public bool SoundEnabled
    {
        get { return PlayerPrefs.GetInt("State.SoundEnabled", 0) == 0; }
        set
        {
            PlayerPrefs.SetInt("State.SoundEnabled", value ? 0 : 1);
            Save();
        }
    }

    public bool SpinDirectionCCW
    {
        get { return PlayerPrefs.GetInt("State.SpinDirection", 0) == 0; }
        set
        {
            PlayerPrefs.SetInt("State.SpinDirection", value ? 0 : 1);
            Save();
        }
    }

    public int GraphicsQuality
    {
        get { return PlayerPrefs.GetInt("State.GraphicsMode", 1); }
        set
        {
            PlayerPrefs.SetInt("State.GraphicsMode", value);
            Save();
        }
    }


    #endregion


    #region Progress


    private Dictionary<string, LevelState> levelStateDic;

    public int JunkCount
    {
        get { return PlayerPrefs.GetInt("State.JunkCount", DefaultJunkCount); }
        private set { PlayerPrefs.SetInt("State.JunkCount", ClampJunkCount(value)); }
    }

    public void ResetProgress()
    {
        foreach (var l in this.levelStateDic)
        {
            l.Value.ResetProgress();
        }
        
        this.JunkCount = DefaultJunkCount;

        Save();
    }

    public int HandleNewJunk(int count)
    {
        count = ClampJunkCount(this.JunkCount + count);

        if (this.JunkCount != count)
        {
            this.JunkCount = count;
        }
        
        Save();
        
        return this.JunkCount;
    }
    
    public LevelState GetLevel(string id)
    {
        if (!this.levelStateDic.ContainsKey(id))
        {
            this.levelStateDic[id] = new LevelState(id);
        }
        
        return this.levelStateDic[id];
    }


    #endregion
    
    
    #region Unity
    
    
    protected override void Awake()
    {
        base.Awake();
        
        this.levelStateDic = new Dictionary<string, LevelState>();
        
        foreach (var l in Data.Instance.Levels)
        {
            this.levelStateDic.Add(l.Id, new LevelState(l.Id));
        }
    }
    
    
    #endregion
    
    
    #region Helpers


    public static string JunkCountToString(int count)
    {
        return string.Format("{0:00000000}", count);
    }

    private static int ClampJunkCount(int count)
    {
        return Mathf.Clamp(count, MinJunkCount, MaxJunkCount);
    }

    public static void Save()
    {
        PlayerPrefs.Save();
    }
    
    
    #endregion
}
