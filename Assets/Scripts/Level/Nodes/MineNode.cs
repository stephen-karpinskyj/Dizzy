using System.Collections.Generic;
using UnityEngine;

public class MineNode : LevelObjectNode
{
    [SerializeField]
    private int numJunkPieces = 4;
    
    private List<JunkController> junk;
    private MineController mine;
    
    
    #region MonoBehaviour
    
    
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(this.transform.position, 0.4f);
    }
    
    
    #endregion
    
    
    #region LevelObjectNode
    
    
    public override LevelObjectNodeLoadResult OnLevelLoad(LevelObjectPool pool)
    {        
        var result = base.OnLevelLoad(pool);
        
        this.mine = pool.Get<MineController>();
        this.mine.transform.parent = this.transform;
        this.mine.transform.localPosition = Vector3.zero;
        this.mine.transform.localRotation = Quaternion.identity;
        
        this.junk = new List<JunkController>();
        for (var i = 0; i < this.numJunkPieces; i++)
        {
            junk.Add(pool.Get<JunkController>());
        }
        this.mine.Initialise(this.junk);
        
        result.Junk = this.junk;
        
        return result;
    }
    
    public override void OnLevelUnload(LevelObjectPool pool)
    {
        pool.Return(this.mine);
        
        foreach (var j in this.junk)
        {
            pool.Return(j);
        }
    }
    
    public override void OnLevelStop()
    {
        base.OnLevelStop();
        
        this.mine.OnLevelStop();
    }
    
    public override void OnLevelStart()
    {
        base.OnLevelStart();
        
        this.mine.OnLevelStart();
    }
    
    
    #endregion
}
