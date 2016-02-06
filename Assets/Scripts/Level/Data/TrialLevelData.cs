using UnityEngine;

public class TrialLevelData : LevelData
{
    [SerializeField, Range(1, 128)]
    private float noviceTime = 25f;
    
    [SerializeField, Range(1, 128)]
    private float proTime = 10f;
    
    public float NoviceTime
    {
        get { return this.noviceTime; }
    }
    
    public float ProTime
    {
        get { return this.proTime; }
    }
}
