using System;
using UnityEngine;

[Serializable]
public class RandomJunkElement
{
    [SerializeField]
    private Renderer rend;

    [SerializeField]
    private int oddsRatio;

    [SerializeField]
    private int junkValue = 1;

    public void SetEnabled(bool enabled)
    {
        this.rend.enabled = enabled;
    }

    public int OddsRatio
    {
        get { return this.oddsRatio; }
    }

    public int JunkValue
    {
        get { return this.junkValue; }
    }
}
