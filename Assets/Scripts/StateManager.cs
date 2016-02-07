using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Manages data and all saved state.
/// </summary>
public class StateManager : BehaviourSingleton<StateManager>
{
    #region Constants
    
    
    private const ulong DefaultJunkCount = 0;
    private const ulong MinJunkCount = DefaultJunkCount;
    private const ulong MaxJunkCount = 999999999999999999;
    
    private const float DefaultJunkMultiplier = 1f;


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

    public int GraphicsMode
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


    private Dictionary<string, TrialLevelState> levelStateDic;

    public ulong JunkCount
    {
        get { return ulong.Parse(PlayerPrefs.GetString("State.JunkCount", "0")); }
        private set { PlayerPrefs.SetString("State.JunkCount", ClampJunkCount(value).ToString()); }
    }
    
    public float JunkMultiplier { get; private set; }

    public void ResetProgress(TrialLevelData data)
    {
        foreach (var l in this.levelStateDic)
        {
            l.Value.ResetProgress(data);
        }
        
        this.JunkCount = DefaultJunkCount;
        this.UpdateJunkMultiplier();

        Save();
    }

    public ulong HandleNewJunk(ulong count)
    {
        count = ClampJunkCount(this.JunkCount + count);

        if (this.JunkCount != count)
        {
            this.JunkCount = count;
        }
        
        Save();
        
        return this.JunkCount;
    }
    
    public TrialLevelState GetLevel(string id)
    {
        if (!this.levelStateDic.ContainsKey(id))
        {
            this.levelStateDic[id] = new TrialLevelState(id);
        }
        
        return this.levelStateDic[id];
    }
    
    public void UpdateJunkMultiplier()
    {
        this.JunkMultiplier = DefaultJunkMultiplier;
        
        foreach (var l in this.levelStateDic)
        {
            this.JunkMultiplier += l.Value.JunkMultiplier;
        }
    }


    #endregion
    
    
    #region Unity
    
    
    protected override void Awake()
    {
        base.Awake();
        
        this.levelStateDic = new Dictionary<string, TrialLevelState>();
        
        foreach (var l in Data.Instance.Levels)
        {
            this.levelStateDic[l.Id] = new TrialLevelState(l.Id);
        }
        
        this.UpdateJunkMultiplier();
    }
    
    
    #endregion
    
    
    #region Helpers


    private static ulong ClampJunkCount(ulong count)
    {
        if (count < MinJunkCount)
        {
            return MinJunkCount;
        }
        
        if (count > MaxJunkCount)
        {
            return MaxJunkCount;
        }
        
        return count;
    }

    public static void Save()
    {
        PlayerPrefs.Save();
    }
    
    
    #endregion
}
