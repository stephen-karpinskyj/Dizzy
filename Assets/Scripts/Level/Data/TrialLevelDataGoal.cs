using System;
using UnityEngine;

[Serializable]
public class TrialLevelDataGoal
{
    [SerializeField, Range(1f, 99.9f)]
    private float time = 20f;
    
    [SerializeField, Range(0.1f, 100f)]
    private float multiplierIncrease = 0.1f;

    public float Time
    {
        get { return this.time; }
    }
    
    public float MultiplierIncrease
    {
        get { return this.multiplierIncrease; }
    }
}
