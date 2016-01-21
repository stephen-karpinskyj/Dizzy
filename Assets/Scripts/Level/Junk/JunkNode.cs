using UnityEngine;

public class JunkNode : LevelObjectNode
{    
    private JunkController junk;
    
    
    #region MonoBehaviour
    
    
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(this.transform.position, 0.12f);
    }
    
    
    #endregion
    
    
    #region LevelObjectNode
    
    
    public override LevelObjectNodeLoadResult OnLevelLoad(LevelObjectPool pool)
    {        
        var result = base.OnLevelLoad(pool);
        
        this.junk = pool.Get<JunkController>();
        
        result.Junk = new JunkController[] { this.junk };
        
        return result;
    }
    
    public override void OnLevelUnload(LevelObjectPool pool)
    {
        pool.Return(this.junk);
    }
    
    public override void OnLevelStop()
    {
        base.OnLevelStop();
        
        this.junk.transform.parent = this.transform;
        this.junk.transform.localPosition = Vector3.zero;
        this.junk.transform.localRotation = Quaternion.identity;

        this.junk.OnLevelStop();
    }
    
    public override void OnLevelStart()
    {
        base.OnLevelStart();
        
        this.junk.OnLevelStart();
    }
    
    
    #endregion
}
