using UnityEngine;

public class ExplorationLevelState : LevelState
{
    public ExplorationLevelState(string levelId)
        : base(levelId)
    {
    }
    
    private string IsBeaconPurchasedPrefKey
    {
        get { return "State.BeaconPurchased." + this.LevelId; }
    }
    
    public bool IsBeaconPurchased
    {
        get { return PlayerPrefs.GetInt(this.IsBeaconPurchasedPrefKey, 0) == 0 ? false : true; }
        private set { PlayerPrefs.SetInt(this.IsBeaconPurchasedPrefKey, value ? 1 : 0); }
    }
    
    public void HandlePurchase()
    {
        this.IsBeaconPurchased = true;
    }
    
    public override void ResetProgress()
    {
        this.IsBeaconPurchased = false;
    }
}
