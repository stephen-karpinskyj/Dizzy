using System;
using UnityEngine;

[Serializable]
public class JunkElement
{
    [SerializeField]
    private Renderer rend;
    
    [SerializeField]
    private Material mat;

    public Renderer Rend
    {
        get { return this.rend; }
    }
    
    public ulong Value { get; set; }
    
    public void Initialise()
    {
        this.rend.material = mat;
    }
}
