using UnityEngine;

/// <summary>
/// Lightweight level object spawner included in level prefab data. 
/// </summary>
public abstract class LevelObjectNode : MonoBehaviour
{
    private Vector3 initialLocalPos;
    private Quaternion initialLocalRot;
    private Vector3 initialLocalScale;
    
    
    #region MonoBehaviour
    
    
    protected virtual void Awake()
    {
        this.initialLocalPos = this.transform.localPosition;
        this.initialLocalRot = this.transform.localRotation;
        this.initialLocalScale = this.transform.localScale;
    }
    
    
    #endregion
    
    
    public virtual LevelObjectNodeLoadResult OnLevelLoad(LevelObjectPool pool) { return default(LevelObjectNodeLoadResult); }
    public virtual void OnLevelUnload(LevelObjectPool pool) { }
    
    public virtual void OnLevelStop()
    {
        this.transform.localPosition = this.initialLocalPos;
        this.transform.localRotation = this.initialLocalRot;
        this.transform.localScale = this.initialLocalScale;
    }
    
    public virtual void OnLevelStart() { }
}
