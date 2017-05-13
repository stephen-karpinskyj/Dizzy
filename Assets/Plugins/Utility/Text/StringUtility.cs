using System;
using System.IO;
using UnityEngine;

public static class StringUtility
{
    /// <summary>
    /// Whether <paramref name="s"/> contains <paramref name="searchString"/> at <paramref name="index"/>.
    /// </summary>
    public static bool IsStringAtIndex(string s, string searchString, ref int index)
    {
        var isStringNext = false;

        if (s[index] == searchString[0])
        {
            var j = index;
            foreach (var c2 in searchString)
            {
                if (c2 == s[j])
                {
                    j++;
                }
                else
                {
                    break;
                }
            }

            if (j == index + searchString.Length)
            {
                isStringNext = true;
                index = j - 1;
            }
        }

        return isStringNext;
    }

    public static string Editor_AbsoluteToRelativeFilepath(string absoluteFilepath)
    {
        #if UNITY_EDITOR
        return "Assets" + absoluteFilepath.Substring(Application.dataPath.Length);
        #else
        throw new NotSupportedException();
        #endif
    }
}
