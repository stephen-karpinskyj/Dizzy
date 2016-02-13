using System;
using UnityEngine;

[Serializable]
public class ExplorationBeaconData
{
    [SerializeField]
    private ulong activationCost;
    
    [SerializeField]
    private Transform target;
    
    public ulong ActivationCost
    {
        get { return this.activationCost; }
    }
    
    public Transform Target
    {
        get { return this.target; }
    }
}
