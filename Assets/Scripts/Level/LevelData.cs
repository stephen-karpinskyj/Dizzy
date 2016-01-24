using UnityEngine;

/// <summary>
/// Level definition data.
/// </summary>
public class LevelData : MonoBehaviour
{
    [SerializeField]
    private string id = "lvl_";
    
    [SerializeField]
    private string displayName;
    
    [SerializeField, Range(1, 128)]
    private float noviceTime = 25f;
    
    [SerializeField, Range(1, 128)]
    private float proTime = 10f;
    
    public string Id
    {
        get { return this.id; }
    }
    
    public string DisplayName
    {
        get { return this.displayName; }
    }
    
    public float NoviceTime
    {
        get { return this.noviceTime; }
    }
    
    public float ProTime
    {
        get { return this.proTime; }
    }
}
