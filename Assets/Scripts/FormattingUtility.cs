using System;
using UnityEngine;

public static class FormattingUtility
{
    public static string JunkCountToString(ulong count, bool optionalDPs = false)
    {
        if (count < 1000)
        {
            return string.Format("{0}", count);
        }
        
        var scaled = 0d;
        var suffix = string.Empty;
        
        if (count < 1000000)
        {
            scaled = count / 1000d;
            suffix = "K";
        }
        else if (count < 1000000000)
        {
            scaled = count / 1000000d;
            suffix = "M";
        }
        else if (count < 1000000000000)
        {
            scaled = count / 1000000000d;
            suffix = "B";
        }
        else if (count < 1000000000000000)
        {
            scaled = count / 1000000000000d;
            suffix = "T";
        }
        else if (count < 1000000000000000000)
        {
            scaled = count / 1000000000000000d;
            suffix = "QD";
        }
        else
        {
            scaled = 999.9d;
            suffix = "QD";
        }
        
        var pow = Math.Log10(scaled);
        var dp = 3 - pow;
        var format = "{0:";
        
        for (var i = 0; i < pow; i++)
        {
            format += "0";
        }
        
        format += ".";
        
        for (var i = 0; i < dp; i++)
        {
            format += optionalDPs ? "#" : "0";
        }
        
        format += "}{1}";
        
        return string.Format(format, scaled, suffix);
    }
    
    public static string SignedJunkCountToString(ulong count)
    {
        return string.Format("+{0}", JunkCountToString(count, true));
    }
    
    public static string JunkMultiplierToString(float multiplier)
    {
        return string.Format("{0:0.0}x", multiplier);
    }
    
    public static string SignedJunkMultiplierToString(float multiplier)
    {
        return string.Format("{0:+0.#;-0.#}x", multiplier);
    }
    
    public static string TimeToString(float time)
    {        
        return string.Format("{0:00.000}", time);
    }
    
    public static string VariableDPTimeToString(float time)
    {
        Debug.Assert(time < 100);
        
        const int MaxDigits = 4;
        var numDp = MaxDigits - ((int)time).ToString().Length;
        
        return time.ToString("F" + numDp);
    }

    public static string TimeDiffToString(float time)
    {
        return string.Format("{0:+0.000;-0.000;0.000}", time);
    }

    public static string RunCountToString(int count)
    {
        return string.Format("{0:0000}", count);
    }
    
    public static string DistanceToString(float distance)
    {
        return string.Format("{0:0.00}AU", distance / 100);
    }
}
