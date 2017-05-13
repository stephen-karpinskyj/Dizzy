using System;
using UnityEngine;

public enum SpecialDirectory
{
    StreamingAssets,
}

public static class SpecialDirectoryExtensions
{
    public static bool IsInUnityProject(this SpecialDirectory sd)
    {
        switch (sd)
        {
            case SpecialDirectory.StreamingAssets: return true;
                
            default: throw new NotImplementedException();
        }
    }

    public static string GetPath(this SpecialDirectory sd)
    {
        switch (sd)
        {
            case SpecialDirectory.StreamingAssets:
            {
                string path = null;
                    
                // TL;DR Only include scheme at runtime for benefit of android which needs to load file from compressed package via www
                #if UNITY_EDITOR
                if (GameUtility.IsPlaying() && Application.platform != RuntimePlatform.WindowsEditor) 
                {
                    path = "file://";
                }
                path += Application.streamingAssetsPath;
                #elif UNITY_ANDROID
                path = "jar:file://" + Application.dataPath + "!/assets";
                #else
                path = "file://" + Application.streamingAssetsPath;
                #endif

                return path;
            }

            default: throw new NotImplementedException();
        }
    }
}
