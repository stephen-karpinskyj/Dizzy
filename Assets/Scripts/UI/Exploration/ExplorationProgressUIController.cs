using UnityEngine;

public class ExplorationProgressUIController : MonoBehaviour
{
    [SerializeField]
    private GameObject parent;
    
    [SerializeField]
    private ExplorationBeaconUIController goal;
    
    public void Show(bool show)
    {
        this.parent.SetActive(show);
    }
    
    public void ForceUpdateAll(ExplorationLevelData data, ExplorationLevelState state, ulong junkCount)
    {
        var isPurchaseable = junkCount > data.Beacon.ActivationCost;
        this.goal.UpdateAll(data.Beacon, state.IsBeaconPurchased, isPurchaseable);
    }
}
