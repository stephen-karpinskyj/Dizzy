using UnityEngine;

public abstract class LevelObjectController : MonoBehaviour
{
    public virtual void OnLevelStop() { }
    public virtual void OnLevelStart() { }
}
