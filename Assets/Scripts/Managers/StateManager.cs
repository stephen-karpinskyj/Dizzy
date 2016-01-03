using System;
using UnityEngine;

/// <summary>
/// Manages data and all saved state.
/// </summary>
public class StateManager : BehaviourSingleton<StateManager>
{
    private const float DefaultTime = 99.999f;
    private const float MinTime = 0f;
    private const float MaxTime = DefaultTime;

    private const int DefaultRunCount = 0;
    private const int MinRunCount = DefaultRunCount;
    private const int MaxRunCount = 9999;

    private const int DefaultJunkCount = 0;
    private const int MinJunkCount = DefaultJunkCount;
    private const int MaxJunkCount = 99999999;

    public static string TimeToString(float time)
    {
        return string.Format("{0:00.000}", time);
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

    public static string JunkCountToString(int count)
    {
        return string.Format("{0:00000000}", count);
    }

    private static int ClampJunkCount(int count)
    {
        return Mathf.Clamp(count, MinJunkCount, MaxJunkCount);
    }

    private static void Save()
    {
        PlayerPrefs.Save();
    }


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


    #endregion


    #region Progress


    public Action<float> OnBestTimeChange;

    public float BestTime
    {
        get { return PlayerPrefs.GetFloat("State.BestTime", DefaultTime); }
        private set
        {
            var time = ClampTime(value);
            PlayerPrefs.SetFloat("State.BestTime", time);

            if (this.OnBestTimeChange != null)
            {
                this.OnBestTimeChange(time);
            }
        }
    }

    public Action<float> OnLastTimeChange;

    public float LastTime
    {
        get { return PlayerPrefs.GetFloat("State.LastTime", DefaultTime); }
        private set
        {
            var time = ClampTime(value);
            PlayerPrefs.SetFloat("State.LastTime", time);

            if (this.OnLastTimeChange != null)
            {
                this.OnLastTimeChange(time);
            }
        }
    }

    public Action<int> OnRunCountChange;

    public int RunCount
    {
        get { return PlayerPrefs.GetInt("State.RunCount", DefaultRunCount); }
        private set
        {
            var count = ClampRunCount(value);
            PlayerPrefs.SetInt("State.RunCount", count);

            if (this.OnRunCountChange != null)
            {
                this.OnRunCountChange(count);
            }
        }
    }

    public Action<int> OnJunkCountChange;

    public int JunkCount
    {
        get { return PlayerPrefs.GetInt("State.JunkCount", DefaultJunkCount); }
        private set
        {
            var count = ClampJunkCount(value);
            PlayerPrefs.SetInt("State.JunkCount", count);

            if (this.OnJunkCountChange != null)
            {
                this.OnJunkCountChange(count);
            }
        }
    }

    public Action<bool> OnNoviceMedalEarntChange;

    public bool NoviceMedalEarnt
    {
        get { return PlayerPrefs.GetInt("State.NoviceMedalEarnt", 0) == 0; }
        private set
        {
            PlayerPrefs.SetInt("State.NoviceMedalEarnt", value ? 0 : 1);

            if (this.OnNoviceMedalEarntChange != null)
            {
                this.OnNoviceMedalEarntChange(value);
            }
        }
    }

    public Action<bool> OnProMedalEarntChange;

    public bool ProMedalEarnt
    {
        get { return PlayerPrefs.GetInt("State.ProMedalEarnt", 0) == 0; }
        private set
        {
            PlayerPrefs.SetInt("State.ProMedalEarnt", value ? 0 : 1);

            if (this.OnProMedalEarntChange != null)
            {
                this.OnProMedalEarntChange(value);
            }
        }
    }

    public void ResetProgress()
    {
        BestTime = DefaultTime;
        LastTime = DefaultTime;
        RunCount = DefaultRunCount;
        JunkCount = DefaultJunkCount;
        NoviceMedalEarnt = false;
        ProMedalEarnt = false;

        Save();
    }

    public void HandleRunComplete(float time, float noviceTime, float proTime)
    {
        time = ClampTime(time);

        if (time < this.BestTime)
        {
            this.BestTime = time;
        }

        this.LastTime = time;

        var newRunCount = ClampRunCount(this.RunCount + 1);

        if (this.RunCount != newRunCount)
        {
            this.RunCount = newRunCount;
        }

        if (time <= noviceTime && !this.NoviceMedalEarnt)
        {
            this.NoviceMedalEarnt = true;
        }

        if (time <= proTime && !this.ProMedalEarnt)
        {
            this.ProMedalEarnt = true;
        }
        
        Save();
    }

    public void HandleNewJunk(int count)
    {
        count = ClampJunkCount(this.JunkCount + count);

        if (this.JunkCount != count)
        {
            this.JunkCount = count;
        }
        
        Save();
    }


    #endregion
}
