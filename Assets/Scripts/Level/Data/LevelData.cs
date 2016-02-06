using UnityEngine;

/// <summary>
/// Level definition data.
/// </summary>
public abstract class LevelData : MonoBehaviour
{
    [SerializeField]
    private string id = "lvl_";
    
    [SerializeField]
    private string displayName;
    
    public string Id
    {
        get { return this.id; }
    }
    
    public string DisplayName
    {
        get { return this.displayName; }
    }
}
