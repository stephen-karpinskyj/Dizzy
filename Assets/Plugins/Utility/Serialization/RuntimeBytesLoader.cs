using System.IO;
using System.Collections;
using UnityEngine;
using System;

public class RuntimeBytesLoader : MonoBehaviour 
{
    public delegate void OnLoadedDelegate(MemoryStream stream);

    public void Load(string filepath, Action<MemoryStream> onLoad)
    {
        Debug.Assert(GameUtility.IsPlaying());
        Debug.Assert(!string.IsNullOrEmpty(filepath), this);
        
        this.StartCoroutine(this.GetTextCoroutine(filepath, onLoad));
    }

    private IEnumerator GetTextCoroutine(string filepath, Action<MemoryStream> onLoaded)
    {
        var www = new WWW(filepath);

        // TODO: Support async
        while (!www.isDone) { }

        onLoaded(new MemoryStream(www.bytes));
        yield return null; // HACK
    }
}
