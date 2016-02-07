using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// All persistent state corresponding to a <see cref="TrialLevelData"/>.
/// </summary>
public class TrialLevelState
{
    #region Constants
    
    
    private const float DefaultTime = 99.999f;
    private const float MinTime = 0f;
    private const float MaxTime = DefaultTime;

    private const int DefaultRunCount = 0;
    private const int MinRunCount = DefaultRunCount;
    private const int MaxRunCount = 9999;
    
    private const int ActiveGoalCount = 2;
    
    
    #endregion
    
    
    #region Fields
    
    
    private string levelId;
    
    private List<TrialLevelDataGoal> activeGoals;
    
    
    #endregion
    
    
    #region Properties
    

    public float BestTime
    {
        get { return PlayerPrefs.GetFloat(this.BestTimePrefKey, DefaultTime); }
        private set { PlayerPrefs.SetFloat(this.BestTimePrefKey, ClampTime(value)); }
    }

    public float LastTime
    {
        get { return PlayerPrefs.GetFloat(LastTimePrefKey, DefaultTime); }
        private set { PlayerPrefs.SetFloat(LastTimePrefKey, ClampTime(value)); }
    }

    public int RunCount
    {
        get { return PlayerPrefs.GetInt(RunCountPrefKey, DefaultRunCount); }
        private set { PlayerPrefs.SetInt(RunCountPrefKey, ClampRunCount(value)); }
    }
    
    public float JunkMultiplier
    {
        get { return PlayerPrefs.GetFloat(JunkMultiplierPrefKey, 0f); }
        private set { PlayerPrefs.SetFloat(JunkMultiplierPrefKey, value); }
    }
    
    public IEnumerable<TrialLevelDataGoal> ActiveGoals
    {
        get { return this.activeGoals; }
    }
    
    
    #endregion
    
    
    #region Initialization
    
    
    public TrialLevelState(string levelId)
    {
        this.levelId = levelId;
    }
    
    
    #endregion
    
    
    #region Private

    
    private string BestTimePrefKey
    {
        get { return "State.BestTime." + this.levelId; }
    }

    private string LastTimePrefKey
    {
        get { return "State.LastTime." + this.levelId; }
    }
    
    private string RunCountPrefKey
    {
        get { return "State.RunCount." + this.levelId; }
    }
    
    private string JunkMultiplierPrefKey
    {
        get { return "State.JunkMultiplier." + this.levelId; }
    }

    
    #endregion
    
    
    #region Public
    
    
    public float HandleWin(float time, TrialLevelData data)
    {
        time = ClampTime(time);

        var newBestTime = time < this.BestTime;

        if (newBestTime)
        {
            this.BestTime = time;
        }

        this.LastTime = time;

        var newRunCount = ClampRunCount(this.RunCount + 1);

        if (this.RunCount != newRunCount)
        {
            this.RunCount = newRunCount;
        }
        
        this.UpdateMultiplierIncrease(data);
        this.UpdateActiveGoals(data);
        
        StateManager.Save();

        return time;
    }
    
    public void ResetProgress(TrialLevelData data)
    {
        this.BestTime = DefaultTime;
        this.LastTime = DefaultTime;
        this.RunCount = DefaultRunCount;
        this.JunkMultiplier = 0f;
        
        this.UpdateWithData(data);
    }
    
    public void UpdateWithData(TrialLevelData data)
    {
        if (data == null)
        {
            return;
        }
        
        this.UpdateMultiplierIncrease(data);
        this.UpdateActiveGoals(data);
    }
    
    
    #endregion
    
    
    #region Helpers
    
    
    private void UpdateMultiplierIncrease(TrialLevelData data)
    {
        this.JunkMultiplier = data.GetCompletedGoalMultiplier(this.BestTime);
    }
    
    private void UpdateActiveGoals(TrialLevelData data)
    {
        this.activeGoals = data.GetActiveGoals(this.BestTime, ActiveGoalCount);
    }
    
    private static float ClampTime(float time)
    {
        return Mathf.Clamp(time, MinTime, MaxTime);
    }

    private static int ClampRunCount(int count)
    {
        return Mathf.Clamp(count, MinRunCount, MaxRunCount);
    }
    
    
    #endregion
}
