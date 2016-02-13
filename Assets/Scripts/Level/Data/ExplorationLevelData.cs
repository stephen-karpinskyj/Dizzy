using UnityEngine;

public class ExplorationLevelData : LevelData
{
    [SerializeField]
    private ExplorationBeaconData beacon;
    
    public ExplorationBeaconData Beacon
    {
        get { return this.beacon; }
    }
}
