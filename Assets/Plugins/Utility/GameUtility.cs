using UnityEngine;

public static class GameUtility
{
    #region Screen


    public static Vector2 GetScreenDimensions()
    {
        #if UNITY_EDITOR
        System.Type T = System.Type.GetType("UnityEditor.GameView,UnityEditor");
        System.Reflection.MethodInfo GetSizeOfMainGameView = T.GetMethod("GetSizeOfMainGameView", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Static);
        System.Object Res = GetSizeOfMainGameView.Invoke(null,null);
        return (Vector2)Res;
        #else
        return new Vector2(Screen.width, Screen.height);
        #endif
    }

    public static float GetScreenAspectRatio()
    {
        var dims = GetScreenDimensions();
        return dims.y / dims.x;
    }


    #endregion


    #region State


    /// <summary>
    /// Whether the game is currently playing, always true if outside editor.
    /// </summary>
    public static bool IsPlaying()
    {
        #if UNITY_EDITOR
        if (!UnityEditor.EditorApplication.isPlaying)
        {
            return false;
        }
        #endif

        return true;
    }


    #endregion
}
