using UnityEngine;

public delegate void OnLevelObjectPickup(LevelObjectController controller);

public abstract class LevelObjectController : MonoBehaviour
{    
    protected OnLevelObjectPickup OnPickup { get; private set; }
    
    public virtual void OnLevelStop(OnLevelObjectPickup onPickup)
    {
        this.OnPickup = onPickup;
    }
    
    public virtual void OnLevelStart()
    {
    }
}
