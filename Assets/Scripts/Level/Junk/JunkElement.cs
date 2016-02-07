using System;
using UnityEngine;

[Serializable]
public class JunkElement
{
    [SerializeField]
    private Renderer rend;
    
    [SerializeField]
    private Material mat;
    
    [SerializeField]
    private GameObject sparkles;

    public Renderer Rend
    {
        get { return this.rend; }
    }
    
    public ulong Value { get; set; }
    
    public void Initialise()
    {
        this.rend.material = mat;
        this.ShowSparkles(false);
    }
    
    public void ShowSparkles(bool show)
    {
        if (this.sparkles)
        {
            this.sparkles.SetActive(show);
        }
    }
}
