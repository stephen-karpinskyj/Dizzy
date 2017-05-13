using UnityEngine;
using System.Collections.Generic;

public static class CollectionExtensions
{
    /// <remarks>Source: http://stackoverflow.com/questions/273313/randomize-a-listt</remarks>
    public static void Shuffle<T>(this IList<T> list)  
    {  
        var n = list.Count;  
        while (n > 1)
        {  
            n--;  
            var k = Random.Range(0, n);  
            T value = list[k];  
            list[k] = list[n];  
            list[n] = value;  
        }  
    }

    /// <remarks>Source: http://stackoverflow.com/a/3261006</remarks>
    public static Vector2I CoordinatesOf<T>(this T[,] matrix, T value)
    {
        int w = matrix.GetLength(0); // width
        int h = matrix.GetLength(1); // height

        for (int x = 0; x < w; ++x)
        {
            for (int y = 0; y < h; ++y)
            {
                if (matrix[x, y].Equals(value))
                    return new Vector2I(x, y);
            }
        }

        return new Vector2I(-1, -1);
    }
}
