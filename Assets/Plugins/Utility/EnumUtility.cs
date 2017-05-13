using System;
using UnityEngine;

using Random = UnityEngine.Random;

public static class EnumUtility
{
    /// <remarks>Source:http://stackoverflow.com/a/5995541</remarks>
    public static T RandomValue<T>() where T : struct
    {
        var t = typeof(T);

        Debug.Assert(t.IsEnum);
        
        var values = Enum.GetValues(t);

        return (T)values.GetValue(Random.Range(0, values.Length));
    }
}
