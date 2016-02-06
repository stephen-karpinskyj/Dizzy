using UnityEngine;

/// <summary>
/// All persistent state corresponding to a level.
/// </summary>
public class LevelState
{
    #region Constants
    
    
    private const float DefaultTime = 99.999f;
    private const float MinTime = 0f;
    private const float MaxTime = DefaultTime;

    private const int DefaultRunCount = 0;
    private const int MinRunCount = DefaultRunCount;
    private const int MaxRunCount = 9999;
    
    
    #endregion
    
    
    #region Fields
    
    
    private string levelId;
    
    
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

    public bool NoviceMedalEarnt
    {
        get { return PlayerPrefs.GetInt(this.NoviceMedalEarntPrefKey, 1) == 0; }
        private set { PlayerPrefs.SetInt(this.NoviceMedalEarntPrefKey, value ? 0 : 1); }
    }

    public bool ProMedalEarnt
    {
        get { return PlayerPrefs.GetInt(this.ProMedalEarntPrefKey, 1) == 0; }
        private set { PlayerPrefs.SetInt(this.ProMedalEarntPrefKey, value ? 0 : 1); }
    }
    
    
    #endregion
    
    
    #region Initialization
    
    
    public LevelState(string levelId)
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
    
    private string NoviceMedalEarntPrefKey
    {
        get { return "State.NoviceMedalEarnt." + this.levelId; }
    }
    
    private string ProMedalEarntPrefKey
    {
        get { return "State.ProMedalEarnt." + this.levelId; }
    }

    
    #endregion
    
    
    #region Public
    
    
    public float HandleWin(float time, LevelData data)
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

        var trialData = data as TrialLevelData;
            
        if (trialData != null)
        {
            if (time <= trialData.NoviceTime && !this.NoviceMedalEarnt)
            {
                this.NoviceMedalEarnt = true;
            }

            if (time <= trialData.ProTime && !this.ProMedalEarnt)
            {
                this.ProMedalEarnt = true;
            }
        }
        
        StateManager.Save();

        return time;
    }
    
    public void ResetProgress()
    {
        this.BestTime = DefaultTime;
        this.LastTime = DefaultTime;
        this.RunCount = DefaultRunCount;
        
        this.NoviceMedalEarnt = false;
        this.ProMedalEarnt = false;
    }
    
    
    #endregion
    
    
    #region Helpers
    
    
    public static string TimeToString(float time)
    {
        return string.Format("{0:00.000}", time);
    }

    public static string TimeDiffToString(float time)
    {
        return string.Format("{0:+0.000;-0.000;0.000}", time);
    }

    private static float ClampTime(float time)
    {
        return Mathf.Clamp(time, MinTime, MaxTime);
    }

    public static string RunCountToString(int count)
    {
        return string.Format("{0:0000}", count);
    }

    private static int ClampRunCount(int count)
    {
        return Mathf.Clamp(count, MinRunCount, MaxRunCount);
    }
    
    
    #endregion
}
